'use client';

import { Check, Copy } from 'lucide-react';
import { useState } from 'react';
import { toast } from 'sonner';

import { useGetSnippetById } from '@/lib/api/endpoints/snippets';
import { useGetProgrammingLanguageEnumValues } from '@/lib/api/endpoints/lookup-data';
import type { TagDto, CollectionSummaryDto } from '@/lib/api/models';

import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { CodeBlock } from './CodeBlock';

interface ViewSnippetDialogProps {
  snippetId: string | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function ViewSnippetDialog({ snippetId, open, onOpenChange }: ViewSnippetDialogProps) {
  const [copied, setCopied] = useState(false);
  const { data: snippet, isLoading } = useGetSnippetById(snippetId || '', { query: { enabled: !!snippetId && open } });
  const { data: languages } = useGetProgrammingLanguageEnumValues();

  const getLanguageName = (languageValue: number) => {
    const language = languages?.find((lang) => lang.value === languageValue);
    return language?.name || 'Unknown';
  };

  const handleCopy = async () => {
    if (!snippet?.content) return;

    try {
      await navigator.clipboard.writeText(snippet.content);
      toast.success('Skopiowano do schowka!');
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch {
      toast.error('Nie udało się skopiować');
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[90vh] max-w-[calc(100%-1rem)] overflow-y-auto p-3 sm:max-w-[50vw] sm:p-6 flex flex-col">
        <DialogHeader>
          <DialogTitle>{snippet?.title || 'Podgląd snippetu'}</DialogTitle>
          <DialogDescription>{snippet?.description || 'Podgląd zawartości snippetu'}</DialogDescription>
        </DialogHeader>

        {/* Language badge below title */}
        {snippet && (
          <div className="pb-2">
            <span className="inline-block px-3 py-1 bg-primary/10 text-primary text-sm font-medium rounded">
              {getLanguageName(snippet.language)}
            </span>
          </div>
        )}

        <div className="flex-1 overflow-y-auto">
          {isLoading ? (
            <div className="flex items-center justify-center py-12">
              <div className="text-muted-foreground">Ładowanie...</div>
            </div>
          ) : snippet ? (
            <div className="space-y-4">
              {/* Tags */}
              {snippet.tags && snippet.tags.length > 0 && (
                <div className="flex flex-wrap gap-2">
                  {snippet.tags.map((tag: TagDto) => (
                    <span
                      key={tag.id}
                      className="px-2 py-1 bg-secondary text-secondary-foreground text-xs rounded"
                      style={tag.color ? { backgroundColor: tag.color + '20', color: tag.color } : undefined}
                    >
                      {tag.name}
                    </span>
                  ))}
                </div>
              )}

              {/* Code with full height */}
              <div className="rounded overflow-hidden">
                <CodeBlock code={snippet.content} language={getLanguageName(snippet.language)} />
              </div>

              {/* Collections */}
              {snippet.collections && snippet.collections.length > 0 && (
                <div className="pt-3 border-t border-border">
                  <p className="text-xs text-muted-foreground mb-2">Kolekcje:</p>
                  <div className="flex flex-wrap gap-2">
                    {snippet.collections.map((collection: CollectionSummaryDto) => (
                      <span key={collection.id} className="text-xs text-muted-foreground">
                        {collection.name}
                      </span>
                    ))}
                  </div>
                </div>
              )}
            </div>
          ) : (
            <div className="flex items-center justify-center py-12">
              <div className="text-muted-foreground">Nie znaleziono snippetu</div>
            </div>
          )}
        </div>

        <div className="flex gap-2 pt-4 border-t">
          <Button variant="outline" onClick={() => onOpenChange(false)} className="flex-1">
            Zamknij
          </Button>
          <Button onClick={handleCopy} disabled={!snippet || copied} className="flex-1">
            {copied ? (
              <>
                <Check className="w-4 h-4 mr-2" />
                Skopiowano
              </>
            ) : (
              <>
                <Copy className="w-4 h-4 mr-2" />
                Kopiuj
              </>
            )}
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
