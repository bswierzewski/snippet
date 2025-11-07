'use client';

import { useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { useDeleteCollection } from '@/lib/api/endpoints/collections';
import { useQueryClient } from '@tanstack/react-query';
import type { CollectionDto } from '@/lib/api/models';

interface DeleteCollectionDialogProps {
  collection: CollectionDto | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function DeleteCollectionDialog({ collection, open, onOpenChange }: DeleteCollectionDialogProps) {
  const queryClient = useQueryClient();

  const deleteCollectionMutation = useDeleteCollection({
    mutation: {
      onSuccess: () => {
        // Refresh the collections list
        queryClient.invalidateQueries({ queryKey: ['/api/collections'] });
        // Close dialog
        onOpenChange(false);
      },
      onError: (error) => {
        console.error('Failed to delete collection:', error);
      },
    },
  });

  // Reset error when collection changes or dialog opens
  useEffect(() => {
    if (collection && open) {
      deleteCollectionMutation.reset();
    }
  }, [collection, open]);

  const handleDelete = () => {
    if (collection) {
      deleteCollectionMutation.mutate({ id: collection.id });
    }
  };

  if (!collection) return null;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[440px]">
        <DialogHeader>
          <DialogTitle>Usuń kolekcję</DialogTitle>
        </DialogHeader>

        <div className="space-y-4">
          <p className="text-sm text-gray-600">
            Czy na pewno chcesz usunąć kolekcję <strong className="text-gray-900">{collection.name}</strong>?
          </p>

          {collection.snippetCount > 0 && (
            <p className="text-sm text-amber-600 bg-amber-50 border border-amber-200 rounded-md p-3">
              Ta kolekcja zawiera {collection.snippetCount} snippet
              {collection.snippetCount === 1 ? '' : collection.snippetCount < 5 ? 'y' : 'ów'}.
            </p>
          )}

          {deleteCollectionMutation.isError && (
            <p className="text-sm text-red-600">Nie udało się usunąć kolekcji. Spróbuj ponownie.</p>
          )}

          <div className="flex justify-end gap-3 pt-2">
            <Button
              type="button"
              variant="outline"
              onClick={() => onOpenChange(false)}
              disabled={deleteCollectionMutation.isPending}
            >
              Anuluj
            </Button>
            <Button
              type="button"
              variant="destructive"
              onClick={handleDelete}
              disabled={deleteCollectionMutation.isPending}
            >
              {deleteCollectionMutation.isPending ? 'Usuwanie...' : 'Usuń'}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}
