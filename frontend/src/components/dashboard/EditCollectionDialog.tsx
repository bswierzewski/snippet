'use client';

import { useQueryClient } from '@tanstack/react-query';
import { useEffect, useState } from 'react';

import { useUpdateCollection } from '@/lib/api/endpoints/collections';
import type { CollectionDto } from '@/lib/api/models';

import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';

interface EditCollectionDialogProps {
  collection: CollectionDto | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function EditCollectionDialog({ collection, open, onOpenChange }: EditCollectionDialogProps) {
  const [name, setName] = useState('');
  const queryClient = useQueryClient();

  const updateCollectionMutation = useUpdateCollection({
    mutation: {
      onSuccess: () => {
        // Refresh the collections list
        queryClient.invalidateQueries({ queryKey: ['/api/collections'] });
        // Close dialog
        onOpenChange(false);
      },
      onError: (error) => {
        console.error('Failed to update collection:', error);
      }
    }
  });

  // Update name and reset error when collection changes or dialog opens
  useEffect(() => {
    if (collection && open) {
      setName(collection.name);
      updateCollectionMutation.reset();
    }
  }, [collection, open]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (collection && name.trim()) {
      updateCollectionMutation.mutate({
        id: collection.id,
        data: {
          id: collection.id,
          name: name.trim(),
          description: collection.description,
          color: collection.color,
          icon: collection.icon
        }
      });
    }
  };

  const handleOpenChange = (newOpen: boolean) => {
    // Reset form when closing
    if (!newOpen && collection) {
      setName(collection.name);
    }
    onOpenChange(newOpen);
  };

  if (!collection) return null;

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-[520px]">
        <DialogHeader>
          <DialogTitle>Edytuj kolekcję</DialogTitle>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="edit-collection-name" className="text-sm text-gray-600 mb-2 block">
              Nazwa kolekcji
            </label>
            <Input
              id="edit-collection-name"
              placeholder="np. React Basics"
              value={name}
              onChange={(e) => setName(e.target.value)}
              disabled={updateCollectionMutation.isPending}
              autoFocus
            />
          </div>

          {updateCollectionMutation.isError && (
            <p className="text-sm text-red-600">Nie udało się zaktualizować kolekcji. Spróbuj ponownie.</p>
          )}

          <div className="flex justify-end gap-3 pt-2">
            <Button
              type="button"
              variant="outline"
              onClick={() => handleOpenChange(false)}
              disabled={updateCollectionMutation.isPending}
            >
              Anuluj
            </Button>
            <Button type="submit" disabled={updateCollectionMutation.isPending || !name.trim()}>
              {updateCollectionMutation.isPending ? 'Zapisywanie...' : 'Zapisz'}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}
