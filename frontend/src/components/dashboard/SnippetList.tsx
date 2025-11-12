'use client';

import { DeleteSnippetDialog } from './DeleteSnippetDialog';
import { EditSnippetDialog } from './EditSnippetDialog';
import { ViewSnippetDialog } from './ViewSnippetDialog';
import { SnippetCard } from './SnippetCard';
import { useFilterStore } from '@/lib/store/filterStore';
import { Code, Loader2 } from 'lucide-react';
import { useState } from 'react';

import { searchSnippets, getSearchSnippetsInfiniteQueryKey } from '@/lib/api/endpoints/snippets';
import type { SnippetSummaryDto } from '@/lib/api/models';
import { useInfiniteQuery } from '@tanstack/react-query';

import { useDebounce } from '@/hooks/useDebounce';
import { useInfiniteScroll } from '@/hooks/useInfiniteScroll';

export function SnippetList() {
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [selectedSnippetId, setSelectedSnippetId] = useState<string | null>(null);
  const [selectedSnippetTitle, setSelectedSnippetTitle] = useState('');

  // Get filters from store
  const { searchTerm, selectedTags, selectedLanguages, selectedCollectionId } = useFilterStore();

  // Debounce search term to avoid too many requests
  const debouncedSearchTerm = useDebounce(searchTerm, 500);

  // Build search query parameters (without pagination for query key)
  const searchQueryBase = {
    searchTerm: debouncedSearchTerm || null,
    tags: selectedTags.length > 0 ? selectedTags : null,
    languages: selectedLanguages.length > 0 ? selectedLanguages : null,
    collectionId: selectedCollectionId || null,
    favoritesOnly: null
  };

  // Fetch snippets with infinite query
  const {
    data,
    isLoading: initialLoading,
    isError,
    error,
    hasNextPage,
    isFetchingNextPage,
    fetchNextPage
  } = useInfiniteQuery({
    queryKey: getSearchSnippetsInfiniteQueryKey({ ...searchQueryBase, pageNumber: 1, pageSize: 5 }),
    queryFn: ({ pageParam = 1, signal }) =>
      searchSnippets(
        {
          ...searchQueryBase,
          pageNumber: pageParam as number,
          pageSize: 5
        },
        signal
      ),
    initialPageParam: 1,
    getNextPageParam: (lastPage) => (lastPage.pageNumber < lastPage.totalPages ? lastPage.pageNumber + 1 : undefined),
    enabled: true
  });

  // Flatten all pages into a single array
  const snippets: SnippetSummaryDto[] = data?.pages.flatMap((page) => page.snippets || []) || [];

  // Set up infinite scroll with Intersection Observer
  const loaderRef = useInfiniteScroll({
    onLoadMore: () => fetchNextPage(),
    isLoading: isFetchingNextPage,
    hasMore: hasNextPage ?? false
  });

  const handleEdit = (snippetId: string) => {
    setSelectedSnippetId(snippetId);
    setEditDialogOpen(true);
  };

  const handleDelete = (snippetId: string, snippetTitle: string) => {
    setSelectedSnippetId(snippetId);
    setSelectedSnippetTitle(snippetTitle);
    setDeleteDialogOpen(true);
  };

  const handleView = (snippetId: string) => {
    setSelectedSnippetId(snippetId);
    setViewDialogOpen(true);
  };

  if (initialLoading) {
    return (
      <main className="flex-1 overflow-y-auto bg-background">
        <div className="p-6">
          <div className="text-center text-muted-foreground">Ładowanie...</div>
        </div>
      </main>
    );
  }

  if (isError) {
    return (
      <main className="flex-1 overflow-y-auto bg-background">
        <div className="p-6">
          <div className="text-center text-destructive">
            {error instanceof Error ? error.message : 'Wystąpił błąd podczas ładowania snippetów'}
          </div>
        </div>
      </main>
    );
  }

  // Empty state - no snippets
  if (snippets.length === 0) {
    return (
      <main className="flex-1 overflow-y-auto bg-background">
        <div className="flex flex-col items-center justify-center min-h-[400px] p-6">
          <div className="w-24 h-24 rounded-full bg-muted flex items-center justify-center mb-4">
            <Code className="w-12 h-12 text-muted-foreground" />
          </div>
          <h2 className="text-xl font-semibold text-foreground mb-2">Brak snippetów</h2>
          <p className="text-muted-foreground text-center">Zacznij organizować swój kod tworząc pierwszy snippet</p>
        </div>
      </main>
    );
  }

  // Display snippets
  return (
    <>
      <main className="flex-1 overflow-y-auto bg-background">
        <div className="p-6">
          <div className="flex flex-col gap-4 max-w-5xl mx-auto">
            {snippets.map((snippet) => (
              <SnippetCard key={snippet.id} snippet={snippet} onEdit={handleEdit} onDelete={handleDelete} onView={handleView} />
            ))}
          </div>

          {/* Infinite scroll trigger element */}
          <div ref={loaderRef} className="flex justify-center items-center py-6 mt-4" style={{ minHeight: '40px' }}>
            {isFetchingNextPage && <Loader2 className="w-6 h-6 animate-spin text-muted-foreground" />}
          </div>

          {/* End of list indicator */}
          {!hasNextPage && snippets.length > 0 && (
            <div className="text-center py-6">
              <p className="text-muted-foreground text-sm">Koniec listy</p>
            </div>
          )}
        </div>
      </main>
      <ViewSnippetDialog snippetId={selectedSnippetId} open={viewDialogOpen} onOpenChange={setViewDialogOpen} />
      <EditSnippetDialog snippetId={selectedSnippetId} open={editDialogOpen} onOpenChange={setEditDialogOpen} />
      <DeleteSnippetDialog
        snippetId={selectedSnippetId}
        snippetTitle={selectedSnippetTitle}
        open={deleteDialogOpen}
        onOpenChange={setDeleteDialogOpen}
      />
    </>
  );
}
