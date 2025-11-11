'use client';

import { useState } from 'react';
import { useGetUserSnippets } from '@/lib/api/endpoints/snippets';
import { Code } from 'lucide-react';
import { SnippetCard } from './SnippetCard';
import { EditSnippetDialog } from './EditSnippetDialog';
import { DeleteSnippetDialog } from './DeleteSnippetDialog';

export function SnippetList() {
  const { data: snippets, isLoading, error } = useGetUserSnippets();
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [selectedSnippetId, setSelectedSnippetId] = useState<string | null>(null);
  const [selectedSnippetTitle, setSelectedSnippetTitle] = useState('');

  const handleEdit = (snippetId: string) => {
    setSelectedSnippetId(snippetId);
    setEditDialogOpen(true);
  };

  const handleDelete = (snippetId: string, snippetTitle: string) => {
    setSelectedSnippetId(snippetId);
    setSelectedSnippetTitle(snippetTitle);
    setDeleteDialogOpen(true);
  };

  if (isLoading) {
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
          <div className="text-center text-destructive">Wystąpił błąd podczas ładowania snippetów</div>
        </div>
      </main>
    );
  }

  // Empty state - no snippets
  if (!snippets || snippets.length === 0) {
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

  // Display snippets in a list
  return (
    <>
      <main className="flex-1 overflow-y-auto bg-background">
        <div className="p-6">
          <div className="flex flex-col gap-4 max-w-5xl mx-auto">
            {snippets.map((snippet) => (
              <SnippetCard key={snippet.id} snippet={snippet} onEdit={handleEdit} onDelete={handleDelete} />
            ))}
          </div>
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
