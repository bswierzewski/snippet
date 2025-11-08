'use client';

import { CreateCollectionDialog } from './CreateCollectionDialog';
import { DeleteCollectionDialog } from './DeleteCollectionDialog';
import { EditCollectionDialog } from './EditCollectionDialog';
import { Folder, Pencil, Plus, Trash2 } from 'lucide-react';
import { useState } from 'react';

import { useGetUserCollections } from '@/lib/api/endpoints/collections';
import type { CollectionDto } from '@/lib/api/models';

export function Sidebar() {
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [editingCollection, setEditingCollection] = useState<CollectionDto | null>(null);
  const [deletingCollection, setDeletingCollection] = useState<CollectionDto | null>(null);
  const { data: collections, isLoading } = useGetUserCollections();

  const handleEditClick = (e: React.MouseEvent, collection: CollectionDto) => {
    e.stopPropagation();
    setEditingCollection(collection);
  };

  const handleDeleteClick = (e: React.MouseEvent, collection: CollectionDto) => {
    e.stopPropagation();
    setDeletingCollection(collection);
  };

  return (
    <aside className="w-[280px] bg-gray-50 border-r border-gray-200 flex flex-col">
      {/* Logo/Header */}
      <div className="h-16 px-4 flex items-center border-b border-gray-200">
        <h1 className="text-xl font-semibold text-gray-900">SnippetVault</h1>
      </div>

      {/* Sidebar content */}
      <div className="flex-1 overflow-y-auto p-4">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-sm font-semibold text-gray-600">KOLEKCJE</h2>
          <button
            onClick={() => setIsCreateDialogOpen(true)}
            className="p-1 hover:bg-gray-200 rounded transition-colors"
            aria-label="Add new collection"
          >
            <Plus className="w-4 h-4 text-gray-600" />
          </button>
        </div>

        {/* Collections list */}
        <div className="space-y-1">
          {/* All snippets */}
          <button className="w-full flex items-center gap-2 px-3 py-2 text-sm rounded-md hover:bg-gray-200 transition-colors text-left bg-gray-200">
            <Folder className="w-4 h-4 text-gray-600" />
            <span className="flex-1 text-gray-900">Wszystkie snippety</span>
          </button>

          {/* User collections */}
          {isLoading ? (
            <div className="px-3 py-2 text-sm text-gray-500">Ładowanie...</div>
          ) : collections && collections.length > 0 ? (
            collections.map((collection) => (
              <div key={collection.id} className="group relative">
                <button className="w-full flex items-center gap-2 px-3 py-2 text-sm rounded-md hover:bg-gray-200 transition-colors text-left">
                  <Folder className="w-4 h-4 text-gray-600 shrink-0" />
                  <span className="flex-1 text-gray-900 truncate">{collection.name}</span>
                  <span className="text-xs text-gray-500 group-hover:opacity-0 shrink-0 w-6 text-right">
                    {collection.snippetCount}
                  </span>
                </button>

                {/* Action buttons - shown on hover */}
                <div className="absolute right-2 top-1/2 -translate-y-1/2 hidden group-hover:flex items-center gap-1">
                  <button
                    onClick={(e) => handleEditClick(e, collection)}
                    className="p-1 hover:bg-gray-300 rounded transition-colors"
                    aria-label="Edit collection"
                  >
                    <Pencil className="w-3.5 h-3.5 text-gray-600" />
                  </button>
                  <button
                    onClick={(e) => handleDeleteClick(e, collection)}
                    className="p-1 hover:bg-gray-300 rounded transition-color"
                    aria-label="Delete collection"
                  >
                    <Trash2 className="w-3.5 h-3.5 text-gray-600" />
                  </button>
                </div>
              </div>
            ))
          ) : (
            <div className="px-3 py-2 text-sm text-gray-500">Brak kolekcji</div>
          )}
        </div>
      </div>

      <CreateCollectionDialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen} />
      <EditCollectionDialog
        collection={editingCollection}
        open={editingCollection !== null}
        onOpenChange={(open) => !open && setEditingCollection(null)}
      />
      <DeleteCollectionDialog
        collection={deletingCollection}
        open={deletingCollection !== null}
        onOpenChange={(open) => !open && setDeletingCollection(null)}
      />
    </aside>
  );
}
