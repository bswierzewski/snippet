'use client';

import { useState, useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { useCreateCollection } from '@/lib/api/endpoints/collections';
import { useQueryClient } from '@tanstack/react-query';

interface CreateCollectionDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function CreateCollectionDialog({ open, onOpenChange }: CreateCollectionDialogProps) {
  const [name, setName] = useState('');
  const queryClient = useQueryClient();

  const createCollectionMutation = useCreateCollection({
    mutation: {
      onSuccess: () => {
        // Refresh the collections list
        queryClient.invalidateQueries({ queryKey: ['/api/collections'] });
        // Close dialog and reset form
        onOpenChange(false);
        setName('');
      },
      onError: (error) => {
        console.error('Failed to create collection:', error);
      },
    },
  });

  // Reset error when dialog opens
  useEffect(() => {
    if (open) {
      createCollectionMutation.reset();
    }
  }, [open]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (name.trim()) {
      createCollectionMutation.mutate({
        data: {
          name: name.trim(),
          description: null,
          color: null,
          icon: null,
        },
      });
    }
  };

  const handleOpenChange = (newOpen: boolean) => {
    // Reset form when closing
    if (!newOpen) {
      setName('');
    }
    onOpenChange(newOpen);
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-[520px]">
        <DialogHeader>
          <DialogTitle>Nowa kolekcja</DialogTitle>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="collection-name" className="text-sm text-gray-600 mb-2 block">
              Nazwa kolekcji
            </label>
            <Input
              id="collection-name"
              placeholder="np. React Basics"
              value={name}
              onChange={(e) => setName(e.target.value)}
              disabled={createCollectionMutation.isPending}
              autoFocus
            />
          </div>

          {createCollectionMutation.isError && (
            <p className="text-sm text-red-600">Nie udało się utworzyć kolekcji. Spróbuj ponownie.</p>
          )}

          <div className="flex justify-end gap-3 pt-2">
            <Button
              type="button"
              variant="outline"
              onClick={() => handleOpenChange(false)}
              disabled={createCollectionMutation.isPending}
            >
              Anuluj
            </Button>
            <Button type="submit" disabled={createCollectionMutation.isPending || !name.trim()}>
              {createCollectionMutation.isPending ? 'Zapisywanie...' : 'Zapisz'}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}
