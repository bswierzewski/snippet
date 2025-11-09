'use client';

import { Check, ChevronDown, Copy, Edit2, Trash2 } from 'lucide-react';
import { useState } from 'react';
import { toast } from 'sonner';

import { useGetProgrammingLanguageEnumValues } from '@/lib/api/endpoints/lookup-data';
import type { SnippetSummaryDto } from '@/lib/api/models';

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
    <div className="bg-white border border-gray-200 rounded-lg p-4 hover:shadow-md transition-shadow">
      {/* Header with title, language and actions */}
      <div className="flex items-start justify-between mb-3">
        <h3 className="text-lg font-semibold text-gray-900 flex-1">{snippet.title}</h3>
        <div className="flex items-center gap-2 ml-4">
          <span className="px-3 py-1 bg-blue-100 text-blue-800 text-sm font-medium rounded">
            {getLanguageName(snippet.language)}
          </span>
          <button
            onClick={handleCopy}
            className={
              copied
                ? 'p-1.5 text-green-600 bg-green-50 rounded transition-colors'
                : 'p-1.5 text-gray-500 hover:text-green-600 hover:bg-green-50 rounded transition-colors'
            }
            aria-label="Copy snippet"
          >
            {copied ? <Check className="w-4 h-4" /> : <Copy className="w-4 h-4" />}
          </button>
          <button
            onClick={handleEdit}
            className="p-1.5 text-gray-500 hover:text-gray-700 hover:bg-gray-100 rounded transition-colors"
            aria-label="Edit snippet"
          >
            <Edit2 className="w-4 h-4" />
          </button>
          <button
            onClick={handleDelete}
            className="p-1.5 text-gray-500 hover:text-red-600 hover:bg-red-50 rounded transition-colors"
            aria-label="Delete snippet"
          >
            <Trash2 className="w-4 h-4" />
          </button>
        </div>
      </div>

      {/* Description */}
      <p className="text-sm text-gray-600 mb-3">
        {snippet.description || <span className="text-gray-400 italic">Brak opisu</span>}
      </p>

      {/* Tags */}
      <div className="flex flex-wrap gap-2 mb-3">
        {snippet.tags.length > 0 ? (
          snippet.tags.map((tag) => (
            <span
              key={tag.id}
              className="px-2 py-1 bg-gray-100 text-gray-700 text-xs rounded"
              style={tag.color ? { backgroundColor: tag.color + '20', color: tag.color } : undefined}
            >
              {tag.name}
            </span>
          ))
        ) : (
          <span className="px-2 py-1 bg-gray-100 text-gray-400 text-xs rounded italic">Brak tagów</span>
        )}
      </div>

      {/* Code preview */}
      <div className="mb-3">
        <div className="bg-gray-50 rounded-t p-3 border border-gray-200">
          <pre className="text-xs font-mono text-gray-800 overflow-x-auto whitespace-pre">
            {isExpanded ? snippet.content : getContentPreview()}
          </pre>
        </div>
        {hasMoreLines() && (
          <div
            onClick={() => setIsExpanded(!isExpanded)}
            className="bg-gray-50 border-t border-x border-b border-gray-300 rounded-b px-3 py-2 flex justify-center items-center gap-1 cursor-pointer hover:bg-gray-100 transition-colors"
          >
            <span className="text-xs text-gray-600">{isExpanded ? 'Zwiń' : 'Rozwiń'}</span>
            <ChevronDown className={`w-3 h-3 text-gray-600 transition-transform ${isExpanded ? 'rotate-180' : ''}`} />
          </div>
        )}
      </div>

      {/* Collections */}
      <div className="pt-3 border-t border-gray-100">
        <p className="text-xs text-gray-500 mb-1">Kolekcje:</p>
        <div className="flex flex-wrap gap-2">
          {snippet.collections.length > 0 ? (
            snippet.collections.map((collection) => (
              <span key={collection.id} className="text-xs text-gray-600">
                {collection.name}
              </span>
            ))
          ) : (
            <span className="text-xs text-gray-400 italic">Brak kolekcji</span>
          )}
        </div>
      </div>
    </div>
  );
}
