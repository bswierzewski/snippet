'use client';

import { useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';

import { useDeleteSnippet, getSearchSnippetsInfiniteQueryKey } from '@/lib/api/endpoints/snippets';

import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle
} from '@/components/ui/dialog';
import { Spinner } from '@/components/ui/spinner';

interface DeleteSnippetDialogProps {
  snippetId: string | null;
  snippetTitle: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function DeleteSnippetDialog({ snippetId, snippetTitle, open, onOpenChange }: DeleteSnippetDialogProps) {
  const queryClient = useQueryClient();
  const { mutateAsync: deleteSnippet, isPending } = useDeleteSnippet();

  const handleDelete = async () => {
    if (!snippetId) return;

    try {
      await deleteSnippet({ id: snippetId });

      // Invalidate queries to refresh lists (use prefix to match all search queries)
      queryClient.invalidateQueries({ queryKey: getSearchSnippetsInfiniteQueryKey().slice(0, 2) });

      // Show success message
      toast.success('Snippet został usunięty');

      // Close dialog
      onOpenChange(false);
    } catch (error) {
      console.error('Error deleting snippet:', error);
      toast.error('Wystąpił błąd podczas usuwania snippetu');
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Usuń snippet</DialogTitle>
          <DialogDescription>
            Czy na pewno chcesz usunąć snippet <strong>{snippetTitle}</strong>? Ta operacja jest nieodwracalna.
          </DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <Button type="button" variant="outline" onClick={() => onOpenChange(false)} disabled={isPending}>
            Anuluj
          </Button>
          <Button type="button" variant="destructive" onClick={handleDelete} disabled={isPending}>
            {isPending ? (
              <>
                <Spinner className="mr-2" />
                Usuwanie...
              </>
            ) : (
              'Usuń'
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
