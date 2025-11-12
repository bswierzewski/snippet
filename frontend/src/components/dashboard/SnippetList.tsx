'use client';

import { useState, useEffect, useRef } from 'react';
import { useVirtualizer } from '@tanstack/react-virtual';
import { useSearchSnippets } from '@/lib/api/endpoints/snippets';
import { useFilterStore } from '@/lib/store/filterStore';
import { usePaginationStore } from '@/lib/store/paginationStore';
import { useDebounce } from '@/hooks/useDebounce';
import { Code, Loader2 } from 'lucide-react';
import { SnippetCard } from './SnippetCard';
import { EditSnippetDialog } from './EditSnippetDialog';
import { DeleteSnippetDialog } from './DeleteSnippetDialog';
import type { SnippetSummaryDto } from '@/lib/api/models';

const ESTIMATE_SIZE = 150; // Approximate height of SnippetCard in pixels

export function SnippetList() {
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [selectedSnippetId, setSelectedSnippetId] = useState<string | null>(null);
  const [selectedSnippetTitle, setSelectedSnippetTitle] = useState('');
  const [snippets, setSnippets] = useState<SnippetSummaryDto[]>([]);
  const [initialLoading, setInitialLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const parentRef = useRef<HTMLDivElement>(null);

  // Get filters from store
  const { searchTerm, selectedTags, selectedLanguages, selectedCollectionId } = useFilterStore();

  // Get pagination state
  const { currentPage, pageSize, hasMore, isFetching, incrementPage, resetPagination, setTotalItems, setHasMore, setIsFetching } = usePaginationStore();

  // Debounce search term to avoid too many requests
  const debouncedSearchTerm = useDebounce(searchTerm, 500);

  // Fetch snippets with filters applied
  const { mutate: searchSnippets, isPending } = useSearchSnippets();

  // Fetch snippets when page or filters change
  useEffect(() => {
    setIsFetching(true);

    searchSnippets(
      {
        data: {
          searchTerm: debouncedSearchTerm || null,
          tags: selectedTags.length > 0 ? selectedTags : null,
          languages: selectedLanguages.length > 0 ? selectedLanguages : null,
          collectionId: selectedCollectionId || null,
          favoritesOnly: null,
          pageNumber: currentPage,
          pageSize: pageSize
        }
      },
      {
        onSuccess: (data) => {
          // If it's the first page (filters changed), reset snippets
          if (currentPage === 1) {
            setSnippets(data.snippets || []);
          } else {
            // Append new snippets to existing ones
            setSnippets((prev) => [...prev, ...(data.snippets || [])]);
          }

          // Update total items and hasMore
          setTotalItems(data.totalCount || 0);
          setHasMore((currentPage * pageSize) < (data.totalCount || 0));

          setError(null);
          setInitialLoading(false);
          setIsFetching(false);
        },
        onError: () => {
          setError('Wystąpił błąd podczas ładowania snippetów');
          setInitialLoading(false);
          setIsFetching(false);
        }
      }
    );
  }, [debouncedSearchTerm, selectedTags, selectedLanguages, selectedCollectionId, currentPage, pageSize, searchSnippets, setTotalItems, setHasMore, setIsFetching]);

  // Reset pagination when filters change
  useEffect(() => {
    resetPagination();
    setSnippets([]);
    setError(null);
  }, [debouncedSearchTerm, selectedTags, selectedLanguages, selectedCollectionId, resetPagination]);

  // Setup virtualizer
  // eslint-disable-next-line react-hooks/incompatible-library
  const virtualizer = useVirtualizer({
    count: snippets.length,
    getScrollElement: () => parentRef.current,
    estimateSize: () => ESTIMATE_SIZE,
    overscan: 10,
  });

  const virtualItems = virtualizer.getVirtualItems();

  // Check if we need to load more items
  useEffect(() => {
    if (!virtualItems.length) return;

    const lastVirtualItem = virtualItems[virtualItems.length - 1];
    if (lastVirtualItem && lastVirtualItem.index >= snippets.length - 3 && hasMore && !isFetching && !isPending) {
      incrementPage();
    }
  }, [virtualItems, snippets.length, hasMore, isFetching, isPending, incrementPage]);

  const handleEdit = (snippetId: string) => {
    setSelectedSnippetId(snippetId);
    setEditDialogOpen(true);
  };

  const handleDelete = (snippetId: string, snippetTitle: string) => {
    setSelectedSnippetId(snippetId);
    setSelectedSnippetTitle(snippetTitle);
    setDeleteDialogOpen(true);
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

  if (error) {
    return (
      <main className="flex-1 overflow-y-auto bg-background">
        <div className="p-6">
          <div className="text-center text-destructive">{error}</div>
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
          <p className="text-muted-foreground text-center">
            Zacznij organizować swój kod tworząc pierwszy snippet
          </p>
        </div>
      </main>
    );
  }

  // Display snippets with virtualizer
  return (
    <>
      <main ref={parentRef} className="flex-1 overflow-y-auto bg-background">
        <div className="p-6">
          <div
            className="flex flex-col gap-4 max-w-5xl mx-auto"
            style={{
              height: `${virtualizer.getTotalSize()}px`,
              width: '100%',
              position: 'relative'
            }}
          >
            {virtualItems.map((virtualItem) => (
              <div
                key={virtualItem.key}
                data-index={virtualItem.index}
                style={{
                  position: 'absolute',
                  top: 0,
                  left: 0,
                  width: '100%',
                  transform: `translateY(${virtualItem.start}px)`
                }}
              >
                <SnippetCard
                  snippet={snippets[virtualItem.index]}
                  onEdit={handleEdit}
                  onDelete={handleDelete}
                />
              </div>
            ))}
          </div>

          {/* Loading indicator for pagination */}
          {isFetching && (
            <div className="flex justify-center items-center py-6 mt-4">
              <Loader2 className="w-6 h-6 animate-spin text-muted-foreground" />
            </div>
          )}

          {/* End of list indicator */}
          {!hasMore && snippets.length > 0 && (
            <div className="text-center py-6 mt-4">
              <p className="text-muted-foreground text-sm">Koniec listy</p>
            </div>
          )}
        </div>
      </main>
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
