'use client';

import { Check, ChevronsUpDown, Plus, X } from 'lucide-react';
import { useState } from 'react';
import { toast } from 'sonner';

import { useCreateTag, useSearchTags } from '@/lib/api/endpoints/tags';

import { useDebounce } from '@/hooks/useDebounce';

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

export interface TagItem {
  id: string;
  name: string;
  color: string | null;
}

interface TagSelectorProps {
  selectedTags: TagItem[];
  onTagsChange: (tags: TagItem[]) => void;
  label?: string;
}

interface SelectedTagsListProps {
  tags: TagItem[];
  onRemoveTag: (tag: TagItem) => void;
}

function SelectedTagsList({ tags, onRemoveTag }: SelectedTagsListProps) {
  if (tags.length === 0) {
    return null;
  }

  return (
    <div className="flex flex-wrap gap-2 pt-2">
      {tags.map((tag) => (
        <button
          key={tag.id}
          type="button"
          onClick={() => onRemoveTag(tag)}
          className="inline-flex items-center gap-1 px-2 py-1 bg-blue-100 text-blue-800 rounded-md text-sm hover:bg-blue-200 transition-colors cursor-pointer"
          aria-label={`Usuń tag ${tag.name}`}
        >
          {tag.name}
          <X className="w-3 h-3" />
        </button>
      ))}
    </div>
  );
}

export function TagSelector({ selectedTags, onTagsChange, label = 'Tagi' }: TagSelectorProps) {
  const [open, setOpen] = useState(false);
  const [searchValue, setSearchValue] = useState('');
  const debouncedSearchTerm = useDebounce(searchValue, 300);

  const { data: searchResults } = useSearchTags(
    { searchTerm: debouncedSearchTerm },
    {
      query: {
        enabled: open,
        placeholderData: (previousData) => previousData
      }
    }
  );

  const { mutateAsync: createTag, isPending: isCreatingTag } = useCreateTag();

  const handleAddTag = (tag: TagItem) => {
    if (!selectedTags.some((t) => t.id === tag.id)) {
      onTagsChange([...selectedTags, tag]);
    }
    setSearchValue('');
  };

  const handleRemoveTag = (tagToRemove: TagItem) => {
    onTagsChange(selectedTags.filter((tag) => tag.id !== tagToRemove.id));
  };

  const handleCreateNewTag = async () => {
    const trimmedValue = searchValue.trim();
    if (!trimmedValue || isCreatingTag) return;

    // Check if tag already exists
    if (exactMatch) {
      toast.error('Tag o tej nazwie już istnieje lub został dodany.');
      return;
    }

    try {
      const newTagData = { name: trimmedValue, color: null };
      const newTagId = await createTag({ data: newTagData });

      const newTag: TagItem = { id: newTagId, ...newTagData };

      onTagsChange([...selectedTags, newTag]);
      setSearchValue('');
      setOpen(false);
      toast.success('Tag utworzony pomyślnie!');
    } catch (error) {
      console.error('Error creating tag:', error);
      toast.error('Wystąpił błąd podczas tworzenia taga');
    }
  };

  // Filter out already selected tags from results and limit to 10
  const filteredResults =
    searchResults?.filter((result) => !selectedTags.some((t) => t.id === result.id)).slice(0, 10) || [];

  // Check for exact match
  const exactMatch =
    searchResults?.some((result) => result.name.toLowerCase() === searchValue.trim().toLowerCase()) ||
    selectedTags.some((tag) => tag.name.toLowerCase() === searchValue.trim().toLowerCase());

  const showCreateOption = searchValue.trim().length > 0 && !exactMatch;

  // Prevent cmdk from capturing text selection shortcuts
  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    // Allow text selection shortcuts: Shift+Home, Shift+End, Ctrl+A, etc.
    if (
      e.key === 'Home' ||
      e.key === 'End' ||
      (e.key === 'a' && (e.ctrlKey || e.metaKey))
    ) {
      e.stopPropagation();
    }
  };

  return (
    <div className="space-y-2">
      <FieldLabel>{label}</FieldLabel>
      <Popover open={open} onOpenChange={setOpen}>
        <PopoverTrigger asChild>
          <Button
            variant="outline"
            role="combobox"
            aria-expanded={open}
            className="w-full justify-between"
          >
            Wybierz tagi...
            <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
          </Button>
        </PopoverTrigger>
        <PopoverContent className="w-full p-0" align="start">
          <Command shouldFilter={false}>
            <CommandInput
              placeholder="Wyszukaj tag..."
              value={searchValue}
              onValueChange={setSearchValue}
              onKeyDown={handleKeyDown}
            />
            <CommandList>
              <CommandEmpty>Brak wyników</CommandEmpty>
              {showCreateOption && (
                <CommandGroup heading="Utwórz nowy">
                  <CommandItem
                    onSelect={handleCreateNewTag}
                    className="text-blue-600"
                    disabled={isCreatingTag}
                  >
                    <Plus className="mr-2 h-4 w-4" />
                    {isCreatingTag ? 'Tworzenie...' : `Utwórz tag "${searchValue.trim()}"`}
                  </CommandItem>
                </CommandGroup>
              )}
              {filteredResults.length > 0 && (
                <CommandGroup heading="Dostępne tagi">
                  {filteredResults.map((result) => (
                    <CommandItem
                      key={result.id}
                      value={result.id}
                      onSelect={() => {
                        handleAddTag({
                          id: result.id,
                          name: result.name,
                          color: null
                        });
                        setOpen(false);
                      }}
                    >
                      <Check
                        className={cn(
                          'mr-2 h-4 w-4',
                          selectedTags.some((t) => t.id === result.id) ? 'opacity-100' : 'opacity-0'
                        )}
                      />
                      {result.name}
                    </CommandItem>
                  ))}
                </CommandGroup>
              )}
            </CommandList>
          </Command>
        </PopoverContent>
      </Popover>

      <SelectedTagsList tags={selectedTags} onRemoveTag={handleRemoveTag} />
    </div>
  );
}
