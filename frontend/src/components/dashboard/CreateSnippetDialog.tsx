'use client';

import { useForm } from '@tanstack/react-form';
import { useQueryClient } from '@tanstack/react-query';
import { X } from 'lucide-react';
import { useState } from 'react';
import { toast } from 'sonner';
import { z } from 'zod';

import { getGetUserCollectionsQueryKey, useGetUserCollections } from '@/lib/api/endpoints/collections';
import { useGetProgrammingLanguageEnumValues } from '@/lib/api/endpoints/lookup-data';
import { getGetUserSnippetsQueryKey, useAddTag, useCreateSnippet } from '@/lib/api/endpoints/snippets';
import type { ProgrammingLanguage } from '@/lib/api/models';

import { Button } from '@/components/ui/button';
import { Checkbox } from '@/components/ui/checkbox';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Field, FieldError, FieldGroup, FieldLabel } from '@/components/ui/field';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Spinner } from '@/components/ui/spinner';
import { Textarea } from '@/components/ui/textarea';

interface CreateSnippetDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

const createSnippetSchema = z
  .object({
    title: z.string().min(1, 'Nazwa jest wymagana'),
    description: z.string(),
    language: z.number().optional(),
    content: z.string().min(1, 'Kod jest wymagany')
  })
  .refine((data) => data.language !== undefined, {
    message: 'Język programowania jest wymagany',
    path: ['language']
  });

type CreateSnippetFormData = z.infer<typeof createSnippetSchema>;

export function CreateSnippetDialog({ open, onOpenChange }: CreateSnippetDialogProps) {
  const [tags, setTags] = useState<string[]>([]);
  const [tagInput, setTagInput] = useState('');
  const [selectedCollections, setSelectedCollections] = useState<string[]>([]);

  const queryClient = useQueryClient();
  const { data: collections, isLoading: isLoadingCollections } = useGetUserCollections();
  const { data: programmingLanguages, isLoading: isLoadingLanguages } = useGetProgrammingLanguageEnumValues();
  const { mutateAsync: createSnippet } = useCreateSnippet();
  const { mutateAsync: addTag } = useAddTag();

  const form = useForm({
    defaultValues: {
      title: '',
      description: '',
      language: undefined as number | undefined,
      content: ''
    } as CreateSnippetFormData,
    validators: {
      onSubmit: createSnippetSchema
    },
    onSubmit: async ({ value }) => {
      try {
        // Create the snippet
        const snippetId = await createSnippet({
          data: {
            title: value.title,
            description: value.description || null,
            language: value.language as ProgrammingLanguage,
            content: value.content,
            collectionIds: selectedCollections.length > 0 ? selectedCollections : null
          }
        });

        // Add tags if any
        if (tags.length > 0 && snippetId) {
          for (const tag of tags) {
            try {
              await addTag({
                id: snippetId,
                data: {
                  snippetId,
                  tagName: tag,
                  color: null
                }
              });
            } catch (tagError) {
              console.error('Error adding tag:', tagError);
            }
          }
        }

        // Invalidate queries to refresh lists
        queryClient.invalidateQueries({ queryKey: getGetUserSnippetsQueryKey() });
        queryClient.invalidateQueries({ queryKey: getGetUserCollectionsQueryKey() });

        // Show success message
        toast.success('Snippet utworzony pomyślnie!');

        // Reset form and close dialog
        resetForm();
        onOpenChange(false);
      } catch (error) {
        console.error('Error creating snippet:', error);
        toast.error('Wystąpił błąd podczas tworzenia snippetu');
      }
    }
  });

  const handleAddTag = () => {
    if (tagInput.trim() && !tags.includes(tagInput.trim())) {
      setTags([...tags, tagInput.trim()]);
      setTagInput('');
    }
  };

  const handleRemoveTag = (tagToRemove: string) => {
    setTags(tags.filter((tag) => tag !== tagToRemove));
  };

  const handleTagInputKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      handleAddTag();
    }
  };

  const handleCollectionToggle = (collectionId: string) => {
    setSelectedCollections((prev) =>
      prev.includes(collectionId) ? prev.filter((id) => id !== collectionId) : [...prev, collectionId]
    );
  };

  const resetForm = () => {
    form.reset();
    setTags([]);
    setSelectedCollections([]);
    setTagInput('');
  };

  const handleCancel = () => {
    resetForm();
    onOpenChange(false);
  };

  const tagSuggestions = ['react', 'hooks', 'state', 'sql', 'database'];

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Nowy snippet</DialogTitle>
        </DialogHeader>

        <form
          id="create-snippet-form"
          onSubmit={(e) => {
            e.preventDefault();
            form.handleSubmit();
          }}
        >
          <FieldGroup>
            <form.Field name="title">
              {(field) => {
                const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
                return (
                  <Field data-invalid={isInvalid}>
                    <FieldLabel htmlFor="title">Nazwa</FieldLabel>
                    <Input
                      id="title"
                      name="title"
                      placeholder="np. React useState Hook"
                      value={field.state.value}
                      onChange={(e) => field.handleChange(e.target.value)}
                      onBlur={field.handleBlur}
                      aria-invalid={isInvalid}
                    />
                    {isInvalid && <FieldError errors={field.state.meta.errors} />}
                  </Field>
                );
              }}
            </form.Field>

            <form.Field name="description">
              {(field) => {
                const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
                return (
                  <Field data-invalid={isInvalid}>
                    <FieldLabel htmlFor="description">Opis</FieldLabel>
                    <Input
                      id="description"
                      name="description"
                      placeholder="Krótki opis snippetu"
                      value={field.state.value}
                      onChange={(e) => field.handleChange(e.target.value)}
                      onBlur={field.handleBlur}
                      aria-invalid={isInvalid}
                    />
                    {isInvalid && <FieldError errors={field.state.meta.errors} />}
                  </Field>
                );
              }}
            </form.Field>

            <form.Field name="language">
              {(field) => {
                const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
                return (
                  <Field data-invalid={isInvalid}>
                    <FieldLabel htmlFor="language">Język programowania</FieldLabel>
                    <Select
                      value={field.state.value !== undefined ? String(field.state.value) : undefined}
                      onValueChange={(value) => {
                        field.handleChange(value ? Number(value) : undefined);
                        field.handleBlur();
                      }}
                      disabled={isLoadingLanguages}
                    >
                      <SelectTrigger id="language" aria-invalid={isInvalid}>
                        <SelectValue placeholder={isLoadingLanguages ? 'Ładowanie...' : 'Wybierz język'} />
                      </SelectTrigger>
                      <SelectContent>
                        {programmingLanguages?.map((lang) => (
                          <SelectItem key={lang.value} value={String(lang.value)}>
                            {lang.name}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    {isInvalid && <FieldError errors={field.state.meta.errors} />}
                  </Field>
                );
              }}
            </form.Field>

            <form.Field name="content">
              {(field) => {
                const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
                return (
                  <Field data-invalid={isInvalid}>
                    <FieldLabel htmlFor="content">Kod</FieldLabel>
                    <Textarea
                      id="content"
                      name="content"
                      placeholder="Wklej tutaj swój kod..."
                      value={field.state.value}
                      onChange={(e) => field.handleChange(e.target.value)}
                      onBlur={field.handleBlur}
                      aria-invalid={isInvalid}
                      rows={10}
                      className="font-mono text-sm"
                    />
                    {isInvalid && <FieldError errors={field.state.meta.errors} />}
                  </Field>
                );
              }}
            </form.Field>
          </FieldGroup>

          <div className="space-y-2 mt-4">
            <FieldLabel htmlFor="tags">Tagi</FieldLabel>
            <div className="flex gap-2">
              <Input
                id="tags"
                placeholder="Dodaj tag i naciśnij Enter"
                value={tagInput}
                onChange={(e) => setTagInput(e.target.value)}
                onKeyDown={handleTagInputKeyDown}
              />
              <Button type="button" onClick={handleAddTag}>
                Dodaj
              </Button>
            </div>
            {tags.length > 0 && (
              <div className="flex flex-wrap gap-2 mt-2">
                {tags.map((tag) => (
                  <span
                    key={tag}
                    className="inline-flex items-center gap-1 px-2 py-1 bg-blue-100 text-blue-800 rounded-md text-sm"
                  >
                    {tag}
                    <button
                      type="button"
                      onClick={() => handleRemoveTag(tag)}
                      className="hover:text-blue-900"
                      aria-label={`Remove ${tag}`}
                    >
                      <X className="w-3 h-3" />
                    </button>
                  </span>
                ))}
              </div>
            )}
            <p className="text-sm text-gray-500 mt-1">Sugestie: {tagSuggestions.join(', ')}</p>
          </div>

          <div className="space-y-2 mt-4">
            <FieldLabel>Kolekcje</FieldLabel>
            {isLoadingCollections ? (
              <p className="text-sm text-gray-500">Ładowanie kolekcji...</p>
            ) : collections && collections.length > 0 ? (
              <div className="space-y-2 mt-2">
                {collections.map((collection) => (
                  <div key={collection.id} className="flex items-center space-x-2">
                    <Checkbox
                      id={`collection-${collection.id}`}
                      checked={selectedCollections.includes(collection.id)}
                      onCheckedChange={() => handleCollectionToggle(collection.id)}
                    />
                    <label
                      htmlFor={`collection-${collection.id}`}
                      className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70 cursor-pointer"
                    >
                      {collection.name}
                    </label>
                  </div>
                ))}
              </div>
            ) : (
              <p className="text-sm text-gray-500">Brak dostępnych kolekcji</p>
            )}
          </div>
        </form>

        <form.Subscribe selector={(state) => state.isSubmitting}>
          {(isSubmitting) => (
            <div className="flex justify-end gap-2 pt-4">
              <Button type="button" variant="outline" onClick={handleCancel} disabled={isSubmitting}>
                Anuluj
              </Button>
              <Button type="submit" form="create-snippet-form" disabled={isSubmitting}>
                {isSubmitting ? (
                  <>
                    <Spinner className="mr-2" />
                    Zapisywanie...
                  </>
                ) : (
                  'Zapisz'
                )}
              </Button>
            </div>
          )}
        </form.Subscribe>
      </DialogContent>
    </Dialog>
  );
}
