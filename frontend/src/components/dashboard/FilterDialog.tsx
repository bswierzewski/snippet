'use client';

import { useState, useMemo } from 'react';
import { useFilterStore } from '@/lib/store/filterStore';
import { useSearchTags } from '@/lib/api/endpoints/tags';
import { useGetProgrammingLanguageEnumValues } from '@/lib/api/endpoints/lookup-data';
import { useDebounce } from '@/hooks/useDebounce';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogFooter
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { FieldLabel } from '@/components/ui/field';
import { Check, X } from 'lucide-react';
import { cn } from '@/lib/utils';

interface FilterDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function FilterDialog({ open, onOpenChange }: FilterDialogProps) {
  const { selectedTags, setSelectedTags, selectedLanguages, setSelectedLanguages } = useFilterStore();
  const [tagSearchValue, setTagSearchValue] = useState('');
  const debouncedTagSearch = useDebounce(tagSearchValue, 300);

  // Fetch tags
  const { data: availableTags = [] } = useSearchTags(
    { searchTerm: debouncedTagSearch },
    {
      query: {
        enabled: open
      }
    }
  );

  // Fetch programming languages
  const { data: availableLanguages = [] } = useGetProgrammingLanguageEnumValues({
    query: {
      enabled: open
    }
  });

  // Filter tags - exclude already selected ones
  const filteredTags = useMemo(() => {
    return availableTags
      .filter((tag) => !selectedTags.includes(tag.name))
      .slice(0, 20);
  }, [availableTags, selectedTags]);

  const handleAddTag = (tagName: string) => {
    setSelectedTags([...selectedTags, tagName]);
    setTagSearchValue('');
  };

  const handleRemoveTag = (tagName: string) => {
    setSelectedTags(selectedTags.filter((t) => t !== tagName));
  };

  const handleAddLanguage = (languageValue: number) => {
    if (!selectedLanguages.includes(languageValue)) {
      setSelectedLanguages([...selectedLanguages, languageValue]);
    }
  };

  const handleRemoveLanguage = (languageValue: number) => {
    setSelectedLanguages(selectedLanguages.filter((l) => l !== languageValue));
  };

  const handleClear = () => {
    setSelectedTags([]);
    setSelectedLanguages([]);
  };

  const handleClose = () => {
    onOpenChange(false);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Filtry</DialogTitle>
          <DialogDescription>
            Wybierz tagi i języka programowania, aby zawęzić wyniki wyszukiwania
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-6">
          {/* Tags Filter */}
          <div className="space-y-3">
            <FieldLabel>Tagi</FieldLabel>

            {/* Search for tags */}
            <Input
              placeholder="Wyszukaj tagi..."
              value={tagSearchValue}
              onChange={(e) => setTagSearchValue(e.target.value)}
              className="w-full"
            />

            {/* Selected tags */}
            {selectedTags.length > 0 && (
              <div className="flex flex-wrap gap-2 pb-2">
                {selectedTags.map((tag) => (
                  <button
                    key={tag}
                    onClick={() => handleRemoveTag(tag)}
                    className="inline-flex items-center gap-1 px-2 py-1 bg-blue-100 text-blue-800 rounded-md text-sm hover:bg-blue-200 transition-colors"
                  >
                    {tag}
                    <X className="w-3 h-3" />
                  </button>
                ))}
              </div>
            )}

            {/* Available tags */}
            <div className="flex flex-wrap gap-2 max-h-64 overflow-y-auto p-2 border rounded-lg bg-muted/50">
              {filteredTags.length > 0 ? (
                filteredTags.map((tag) => (
                  <button
                    key={tag.id}
                    onClick={() => handleAddTag(tag.name)}
                    className="px-3 py-1 bg-background border border-input rounded-md text-sm hover:bg-accent transition-colors"
                  >
                    {tag.name}
                  </button>
                ))
              ) : (
                <p className="text-sm text-muted-foreground p-2">Brak dostępnych tagów</p>
              )}
            </div>
          </div>

          {/* Languages Filter */}
          <div className="space-y-3">
            <FieldLabel>Języki programowania</FieldLabel>

            {/* Selected languages */}
            {selectedLanguages.length > 0 && (
              <div className="flex flex-wrap gap-2 pb-2">
                {selectedLanguages.map((langValue) => {
                  const lang = availableLanguages.find((l) => l.value === langValue);
                  return (
                    <button
                      key={langValue}
                      onClick={() => handleRemoveLanguage(langValue)}
                      className="inline-flex items-center gap-1 px-2 py-1 bg-purple-100 text-purple-800 rounded-md text-sm hover:bg-purple-200 transition-colors"
                    >
                      {lang?.name || `Language ${langValue}`}
                      <X className="w-3 h-3" />
                    </button>
                  );
                })}
              </div>
            )}

            {/* Available languages */}
            <div className="grid grid-cols-2 gap-2 max-h-64 overflow-y-auto p-2 border rounded-lg bg-muted/50">
              {availableLanguages.map((language) => {
                const isSelected = selectedLanguages.includes(language.value);
                return (
                  <button
                    key={language.value}
                    onClick={() =>
                      isSelected
                        ? handleRemoveLanguage(language.value)
                        : handleAddLanguage(language.value)
                    }
                    className={cn(
                      'flex items-center gap-2 px-3 py-2 rounded-md text-sm transition-colors text-left',
                      isSelected
                        ? 'bg-primary text-primary-foreground'
                        : 'bg-background border border-input hover:bg-accent'
                    )}
                  >
                    <Check
                      className={cn(
                        'w-4 h-4 flex-shrink-0',
                        isSelected ? 'opacity-100' : 'opacity-0'
                      )}
                    />
                    <span>{language.name}</span>
                  </button>
                );
              })}
            </div>
          </div>
        </div>

        <DialogFooter className="flex justify-between">
          <Button variant="outline" onClick={handleClear}>
            Wyczyść filtry
          </Button>
          <Button onClick={handleClose}>
            Gotowe
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
