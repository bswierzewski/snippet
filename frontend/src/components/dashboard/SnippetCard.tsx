'use client';

import { Check, ChevronDown, Copy, Edit2, Trash2 } from 'lucide-react';
import { useState } from 'react';
import { toast } from 'sonner';

import { useGetProgrammingLanguageEnumValues } from '@/lib/api/endpoints/lookup-data';
import type { SnippetSummaryDto } from '@/lib/api/models';
import { CodeBlock } from './CodeBlock';

interface SnippetCardProps {
  snippet: SnippetSummaryDto;
  onEdit?: (snippetId: string) => void;
  onDelete?: (snippetId: string, snippetTitle: string) => void;
}

export function SnippetCard({ snippet, onEdit, onDelete }: SnippetCardProps) {
  const { data: languages } = useGetProgrammingLanguageEnumValues();
  const [copied, setCopied] = useState(false);
  const [isExpanded, setIsExpanded] = useState(false);

  const getLanguageName = (languageValue: number) => {
    const language = languages?.find((lang) => lang.value === languageValue);
    return language?.name || 'Unknown';
  };

  const handleCopy = async () => {
    try {
      await navigator.clipboard.writeText(snippet.content);
      toast.success('Skopiowano do schowka!');
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch (error) {
      toast.error('Nie udało się skopiować');
    }
  };

  const handleEdit = () => {
    onEdit?.(snippet.id);
  };

  const handleDelete = () => {
    onDelete?.(snippet.id, snippet.title);
  };

  // Get preview of content (first 5 lines)
  const getContentPreview = () => {
    const lines = snippet.content.split('\n');
    return lines.slice(0, 5).join('\n');
  };

  const hasMoreLines = () => {
    const lines = snippet.content.split('\n');
    return lines.length > 5;
  };

  return (
    <div className="bg-card border border-border rounded-lg p-4 hover:shadow-md transition-shadow">
      {/* Header with title, language and actions */}
      <div className="flex items-start justify-between mb-3">
        <h3 className="text-lg font-semibold text-card-foreground flex-1">{snippet.title}</h3>
        <div className="flex items-center gap-2 ml-4">
          <span className="px-3 py-1 bg-primary/10 text-primary text-sm font-medium rounded">
            {getLanguageName(snippet.language)}
          </span>
          <button
            onClick={handleCopy}
            className={
              copied
                ? 'p-1.5 text-green-600 bg-green-500/10 rounded transition-colors'
                : 'p-1.5 text-muted-foreground hover:text-green-600 hover:bg-green-500/10 rounded transition-colors'
            }
            aria-label="Copy snippet"
          >
            {copied ? <Check className="w-4 h-4" /> : <Copy className="w-4 h-4" />}
          </button>
          <button
            onClick={handleEdit}
            className="p-1.5 text-muted-foreground hover:text-card-foreground hover:bg-accent rounded transition-colors"
            aria-label="Edit snippet"
          >
            <Edit2 className="w-4 h-4" />
          </button>
          <button
            onClick={handleDelete}
            className="p-1.5 text-muted-foreground hover:text-destructive hover:bg-destructive/10 rounded transition-colors"
            aria-label="Delete snippet"
          >
            <Trash2 className="w-4 h-4" />
          </button>
        </div>
      </div>

      {/* Description */}
      <p className="text-sm text-muted-foreground mb-3">
        {snippet.description || <span className="text-muted-foreground/60 italic">Brak opisu</span>}
      </p>

      {/* Tags */}
      <div className="flex flex-wrap gap-2 mb-3">
        {snippet.tags.length > 0 ? (
          snippet.tags.map((tag) => (
            <span
              key={tag.id}
              className="px-2 py-1 bg-secondary text-secondary-foreground text-xs rounded"
              style={tag.color ? { backgroundColor: tag.color + '20', color: tag.color } : undefined}
            >
              {tag.name}
            </span>
          ))
        ) : (
          <span className="px-2 py-1 bg-secondary text-muted-foreground/60 text-xs rounded italic">Brak tagów</span>
        )}
      </div>

      {/* Code preview */}
      <div className="mb-3">
        <div className="rounded-t overflow-hidden">
          <CodeBlock
            code={isExpanded ? snippet.content : getContentPreview()}
            language={getLanguageName(snippet.language)}
          />
        </div>
        {hasMoreLines() && (
          <div
            onClick={() => setIsExpanded(!isExpanded)}
            className="bg-muted border-t border-border rounded-b px-3 py-2 flex justify-center items-center gap-1 cursor-pointer hover:bg-muted/80 transition-colors"
          >
            <span className="text-xs text-muted-foreground">{isExpanded ? 'Zwiń' : 'Rozwiń'}</span>
            <ChevronDown className={`w-3 h-3 text-muted-foreground transition-transform ${isExpanded ? 'rotate-180' : ''}`} />
          </div>
        )}
      </div>

      {/* Collections */}
      <div className="pt-3 border-t border-border">
        <p className="text-xs text-muted-foreground mb-1">Kolekcje:</p>
        <div className="flex flex-wrap gap-2">
          {snippet.collections.length > 0 ? (
            snippet.collections.map((collection) => (
              <span key={collection.id} className="text-xs text-muted-foreground">
                {collection.name}
              </span>
            ))
          ) : (
            <span className="text-xs text-muted-foreground/60 italic">Brak kolekcji</span>
          )}
        </div>
      </div>
    </div>
  );
}
