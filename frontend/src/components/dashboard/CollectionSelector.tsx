'use client';

import { Check, ChevronsUpDown, Plus, X } from 'lucide-react';
import { useMemo, useState } from 'react';
import { toast } from 'sonner';

import { useCreateCollection, useGetUserCollections } from '@/lib/api/endpoints/collections';

import { Button } from '@/components/ui/button';
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList
} from '@/components/ui/command';
import { FieldLabel } from '@/components/ui/field';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';

import { cn } from '@/lib/utils';

export interface CollectionItem {
  id: string;
  name: string;
  description?: string | null;
}

interface CollectionSelectorProps {
  selectedCollections: CollectionItem[];
  onCollectionsChange: (collections: CollectionItem[]) => void;
  label?: string;
}

interface SelectedCollectionsListProps {
  collections: CollectionItem[];
  onRemoveCollection: (collection: CollectionItem) => void;
}

function SelectedCollectionsList({ collections, onRemoveCollection }: SelectedCollectionsListProps) {
  if (collections.length === 0) {
    return null;
  }

  return (
    <div className="flex flex-wrap gap-2 pt-2">
      {collections.map((collection) => (
        <button
          key={collection.id}
          type="button"
          onClick={() => onRemoveCollection(collection)}
          className="inline-flex items-center gap-1 px-2 py-1 bg-purple-100 text-purple-800 rounded-md text-sm hover:bg-purple-200 transition-colors cursor-pointer"
          aria-label={`Usuń kolekcję ${collection.name}`}
        >
          {collection.name}
          <X className="w-3 h-3" />
        </button>
      ))}
    </div>
  );
}

export function CollectionSelector({
  selectedCollections,
  onCollectionsChange,
  label = 'Kolekcje'
}: CollectionSelectorProps) {
  const [open, setOpen] = useState(false);
  const [searchValue, setSearchValue] = useState('');

  const { data: allCollections, isLoading } = useGetUserCollections({
    query: {
      enabled: open
    }
  });

  const { mutateAsync: createCollection, isPending: isCreatingCollection } = useCreateCollection();

  const handleAddCollection = (collection: CollectionItem) => {
    if (!selectedCollections.some((c) => c.id === collection.id)) {
      onCollectionsChange([...selectedCollections, collection]);
    }
    setSearchValue('');
  };

  const handleRemoveCollection = (collectionToRemove: CollectionItem) => {
    onCollectionsChange(selectedCollections.filter((collection) => collection.id !== collectionToRemove.id));
  };

  const handleCreateNewCollection = async () => {
    const trimmedValue = searchValue.trim();
    if (!trimmedValue || isCreatingCollection) return;

    // Check if collection already exists
    if (exactMatch) {
      toast.error('Kolekcja o tej nazwie już istnieje lub została dodana.');
      return;
    }

    try {
      const newCollectionData = { name: trimmedValue, description: null, color: null, icon: null };
      const newCollectionId = await createCollection({ data: newCollectionData });

      const newCollection: CollectionItem = { id: newCollectionId, name: trimmedValue };

      onCollectionsChange([...selectedCollections, newCollection]);
      setSearchValue('');
      setOpen(false);
      toast.success('Kolekcja utworzona pomyślnie!');
    } catch (error) {
      console.error('Error creating collection:', error);
      toast.error('Wystąpił błąd podczas tworzenia kolekcji');
    }
  };

  // Filter collections based on search value and exclude already selected ones
  const filteredResults = useMemo(() => {
    if (!allCollections) return [];

    const searchLower = searchValue.toLowerCase().trim();
    return allCollections
      .filter((collection) => {
        // Exclude already selected collections
        if (selectedCollections.some((c) => c.id === collection.id)) return false;
        // Filter by search value
        if (searchLower && !collection.name.toLowerCase().includes(searchLower)) return false;
        return true;
      })
      .slice(0, 10);
  }, [allCollections, searchValue, selectedCollections]);

  // Check for exact match
  const exactMatch = useMemo(() => {
    const searchLower = searchValue.trim().toLowerCase();
    return (
      allCollections?.some((collection) => collection.name.toLowerCase() === searchLower) ||
      selectedCollections.some((collection) => collection.name.toLowerCase() === searchLower)
    );
  }, [allCollections, selectedCollections, searchValue]);

  const showCreateOption = searchValue.trim().length > 0 && !exactMatch;

  // Prevent cmdk from capturing text selection shortcuts
  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    // Allow text selection shortcuts: Shift+Home, Shift+End, Ctrl+A, etc.
    if (e.key === 'Home' || e.key === 'End' || (e.key === 'a' && (e.ctrlKey || e.metaKey))) {
      e.stopPropagation();
    }
  };

  return (
    <div className="space-y-2">
      <FieldLabel>{label}</FieldLabel>
      <Popover open={open} onOpenChange={setOpen}>
        <PopoverTrigger asChild>
          <Button variant="outline" role="combobox" aria-expanded={open} className="w-full justify-between">
            {isLoading ? 'Ładowanie...' : 'Wybierz kolekcje...'}
            <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
          </Button>
        </PopoverTrigger>
        <PopoverContent className="w-full p-0" align="start">
          <Command shouldFilter={false}>
            <CommandInput
              placeholder="Wyszukaj kolekcję..."
              value={searchValue}
              onValueChange={setSearchValue}
              onKeyDown={handleKeyDown}
            />
            <CommandList>
              <CommandEmpty>Brak wyników</CommandEmpty>
              {showCreateOption && (
                <CommandGroup heading="Utwórz nową">
                  <CommandItem
                    onSelect={handleCreateNewCollection}
                    className="text-purple-600"
                    disabled={isCreatingCollection}
                  >
                    <Plus className="mr-2 h-4 w-4" />
                    {isCreatingCollection ? 'Tworzenie...' : `Utwórz kolekcję "${searchValue.trim()}"`}
                  </CommandItem>
                </CommandGroup>
              )}
              {filteredResults.length > 0 && (
                <CommandGroup heading="Dostępne kolekcje">
                  {filteredResults.map((result) => (
                    <CommandItem
                      key={result.id}
                      value={result.id}
                      onSelect={() => {
                        handleAddCollection({
                          id: result.id,
                          name: result.name,
                          description: result.description
                        });
                        setOpen(false);
                      }}
                    >
                      <Check
                        className={cn(
                          'mr-2 h-4 w-4',
                          selectedCollections.some((c) => c.id === result.id) ? 'opacity-100' : 'opacity-0'
                        )}
                      />
                      <div className="flex flex-col">
                        <span>{result.name}</span>
                        {result.description && (
                          <span className="text-xs text-gray-500">{result.description}</span>
                        )}
                      </div>
                    </CommandItem>
                  ))}
                </CommandGroup>
              )}
            </CommandList>
          </Command>
        </PopoverContent>
      </Popover>

      <SelectedCollectionsList collections={selectedCollections} onRemoveCollection={handleRemoveCollection} />
    </div>
  );
}
