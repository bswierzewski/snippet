'use client';

import { CreateCollectionDialog } from './CreateCollectionDialog';
import { DeleteCollectionDialog } from './DeleteCollectionDialog';
import { EditCollectionDialog } from './EditCollectionDialog';
import { Feather, Folder, FolderOpen, Pencil, Plus, Trash2 } from 'lucide-react';
import { useState } from 'react';

import { useGetUserCollections } from '@/lib/api/endpoints/collections';
import type { CollectionDto } from '@/lib/api/models';
import { useFilterStore } from '@/lib/store/filterStore';

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
  const { selectedCollectionId, setSelectedCollectionId } = useFilterStore();

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
      <Sidebar collapsible="offcanvas" className="bg-sidebar border-sidebar-border">
        {/* Logo/Header */}
        <SidebarHeader className="h-16 flex items-center justify-center border-b border-sidebar-border">
          <div className="flex items-center gap-3">
            <Feather className="w-8 h-8 text-sidebar-foreground" strokeWidth={2.5} />
            <h1 className="text-3xl font-bold tracking-wider text-sidebar-foreground">
              QUILL
            </h1>
          </div>
        </SidebarHeader>

        {/* Sidebar content */}
        <SidebarContent className="bg-sidebar">
          <SidebarGroup>
            <SidebarGroupLabel className="flex items-center justify-between text-sidebar-foreground/70 font-semibold">
              <span>KOLEKCJE</span>
              <button
                onClick={() => setIsCreateDialogOpen(true)}
                className="p-1 hover:bg-sidebar-accent rounded transition-colors"
                aria-label="Add new collection"
              >
                <Plus className="w-4 h-4 text-sidebar-foreground/70" />
              </button>
            </SidebarGroupLabel>
            <SidebarGroupContent>
              <SidebarMenu>
                {/* All snippets */}
                <SidebarMenuItem>
                  <SidebarMenuButton
                    isActive={selectedCollectionId === null}
                    onClick={() => setSelectedCollectionId(null)}
                    className="hover:bg-sidebar-accent data-[active=true]:bg-sidebar-accent text-sidebar-foreground"
                  >
                    {selectedCollectionId === null ? (
                      <FolderOpen className="w-4 h-4 text-sidebar-foreground/70" />
                    ) : (
                      <Folder className="w-4 h-4 text-sidebar-foreground/70" />
                    )}
                    <span>Wszystkie snippety</span>
                  </SidebarMenuButton>
                </SidebarMenuItem>

                {/* User collections */}
                {isLoading ? (
                  <div className="px-3 py-2 text-sm text-muted-foreground">Ładowanie...</div>
                ) : collections && collections.length > 0 ? (
                  collections.map((collection) => {
                    const isActive = selectedCollectionId === collection.id;
                    return (
                      <SidebarMenuItem key={collection.id} className="group/item relative">
                        <SidebarMenuButton
                          isActive={isActive}
                          onClick={() => setSelectedCollectionId(collection.id)}
                          className="hover:bg-sidebar-accent data-[active=true]:bg-sidebar-accent text-sidebar-foreground"
                        >
                          {isActive ? (
                            <FolderOpen className="w-4 h-4 text-sidebar-foreground/70 shrink-0" />
                          ) : (
                            <Folder className="w-4 h-4 text-sidebar-foreground/70 shrink-0" />
                          )}
                          <span className="flex-1 truncate">{collection.name}</span>
                          <span className="text-xs text-muted-foreground group-hover/item:opacity-0 shrink-0 w-6 text-right">
                            {collection.snippetCount}
                          </span>
                        </SidebarMenuButton>

                        {/* Action buttons - shown on hover */}
                        <div className="absolute right-2 top-1/2 -translate-y-1/2 hidden group-hover/item:flex items-center gap-1">
                          <button
                            onClick={(e) => handleEditClick(e, collection)}
                            className="p-1 hover:bg-sidebar-primary/20 rounded transition-colors"
                            aria-label="Edit collection"
                          >
                            <Pencil className="w-3.5 h-3.5 text-sidebar-foreground" />
                          </button>
                          <button
                            onClick={(e) => handleDeleteClick(e, collection)}
                            className="p-1 hover:bg-destructive/20 rounded transition-color"
                            aria-label="Delete collection"
                          >
                            <Trash2 className="w-3.5 h-3.5 text-sidebar-foreground" />
                          </button>
                        </div>
                      </SidebarMenuItem>
                    );
                  })
                ) : (
                  <div className="px-3 py-2 text-sm text-muted-foreground">Brak kolekcji</div>
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
