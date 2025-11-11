'use client';

import { CreateCollectionDialog } from './CreateCollectionDialog';
import { DeleteCollectionDialog } from './DeleteCollectionDialog';
import { EditCollectionDialog } from './EditCollectionDialog';
import { Feather, Folder, Pencil, Plus, Trash2 } from 'lucide-react';
import { useState } from 'react';

import { useGetUserCollections } from '@/lib/api/endpoints/collections';
import type { CollectionDto } from '@/lib/api/models';

import {
  Sidebar,
  SidebarContent,
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem
} from '@/components/ui/sidebar';

export function AppSidebar() {
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
    <>
      <Sidebar collapsible="offcanvas" className="bg-gray-50 border-gray-200">
        {/* Logo/Header */}
        <SidebarHeader className="h-16 flex items-center justify-center border-b border-gray-200">
          <div className="flex items-center gap-3">
            <Feather className="w-8 h-8 text-gray-900" strokeWidth={2.5} />
            <h1 className="text-3xl font-bold tracking-wider text-gray-900 bg-gradient-to-r from-gray-900 via-gray-700 to-gray-900 bg-clip-text text-transparent">
              QUILL
            </h1>
          </div>
        </SidebarHeader>

        {/* Sidebar content */}
        <SidebarContent className="bg-gray-50">
          <SidebarGroup>
            <SidebarGroupLabel className="flex items-center justify-between text-gray-600 font-semibold">
              <span>KOLEKCJE</span>
              <button
                onClick={() => setIsCreateDialogOpen(true)}
                className="p-1 hover:bg-gray-200 rounded transition-colors"
                aria-label="Add new collection"
              >
                <Plus className="w-4 h-4 text-gray-600" />
              </button>
            </SidebarGroupLabel>
            <SidebarGroupContent>
              <SidebarMenu>
                {/* All snippets */}
                <SidebarMenuItem>
                  <SidebarMenuButton
                    isActive
                    className="hover:bg-gray-200 data-[active=true]:bg-gray-200 text-gray-900"
                  >
                    <Folder className="w-4 h-4 text-gray-600" />
                    <span>Wszystkie snippety</span>
                  </SidebarMenuButton>
                </SidebarMenuItem>

                {/* User collections */}
                {isLoading ? (
                  <div className="px-3 py-2 text-sm text-gray-500">Ładowanie...</div>
                ) : collections && collections.length > 0 ? (
                  collections.map((collection) => (
                    <SidebarMenuItem key={collection.id} className="group/item relative">
                      <SidebarMenuButton className="hover:bg-gray-200 text-gray-900">
                        <Folder className="w-4 h-4 text-gray-600 shrink-0" />
                        <span className="flex-1 truncate">{collection.name}</span>
                        <span className="text-xs text-gray-500 group-hover/item:opacity-0 shrink-0 w-6 text-right">
                          {collection.snippetCount}
                        </span>
                      </SidebarMenuButton>

                      {/* Action buttons - shown on hover */}
                      <div className="absolute right-2 top-1/2 -translate-y-1/2 hidden group-hover/item:flex items-center gap-1">
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
                    </SidebarMenuItem>
                  ))
                ) : (
                  <div className="px-3 py-2 text-sm text-gray-500">Brak kolekcji</div>
                )}
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
        </SidebarContent>
      </Sidebar>

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
    </>
  );
}
