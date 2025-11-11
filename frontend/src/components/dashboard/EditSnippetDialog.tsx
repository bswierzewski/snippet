'use client';

import { useForm } from '@tanstack/react-form';
import { useQueryClient } from '@tanstack/react-query';
import { useEffect, useState } from 'react';
import { toast } from 'sonner';
import { z } from 'zod';

import { getGetUserCollectionsQueryKey } from '@/lib/api/endpoints/collections';
import { useGetProgrammingLanguageEnumValues } from '@/lib/api/endpoints/lookup-data';
import { getGetUserSnippetsQueryKey, useGetSnippetById, useUpdateSnippet } from '@/lib/api/endpoints/snippets';
import type { ProgrammingLanguage } from '@/lib/api/models';

import { type CollectionItem, CollectionSelector } from '@/components/dashboard/CollectionSelector';
import { type TagItem, TagSelector } from '@/components/dashboard/TagSelector';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Field, FieldError, FieldGroup, FieldLabel } from '@/components/ui/field';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Spinner } from '@/components/ui/spinner';
import { Textarea } from '@/components/ui/textarea';

interface EditSnippetDialogProps {
  snippetId: string | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

const updateSnippetSchema = z
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

type UpdateSnippetFormData = z.infer<typeof updateSnippetSchema>;

export function EditSnippetDialog({ snippetId, open, onOpenChange }: EditSnippetDialogProps) {
  const queryClient = useQueryClient();
  const { data: snippet, isLoading: isLoadingSnippet } = useGetSnippetById(snippetId || '', {
    query: {
      enabled: !!snippetId && open,
      refetchOnMount: 'always'
    }
  });
  const { data: programmingLanguages, isLoading: isLoadingLanguages } = useGetProgrammingLanguageEnumValues();
  const { mutateAsync: updateSnippet } = useUpdateSnippet();

  const [tags, setTags] = useState<TagItem[]>(snippet?.tags ?? []);
  const [selectedCollections, setSelectedCollections] = useState<CollectionItem[]>(
    snippet?.collections.map((c) => ({ id: c.id, name: c.name })) ?? []
  );

  const form = useForm({
    defaultValues: {
      title: snippet?.title ?? '',
      description: snippet?.description ?? '',
      language: snippet?.language,
      content: snippet?.content ?? ''
    } as UpdateSnippetFormData,
    validators: {
      onSubmit: updateSnippetSchema
    },
    onSubmit: async ({ value }) => {
      if (!snippetId) return;

      try {
        // Update the snippet
        await updateSnippet({
          id: snippetId,
          data: {
            id: snippetId,
            title: value.title,
            description: value.description || null,
            language: value.language as ProgrammingLanguage,
            content: value.content,
            tagIds: tags.map((t) => t.id),
            collectionIds: selectedCollections.map((c) => c.id)
          }
        });

        // Invalidate queries to refresh lists
        queryClient.invalidateQueries({ queryKey: getGetUserSnippetsQueryKey() });
        queryClient.invalidateQueries({ queryKey: getGetUserCollectionsQueryKey() });

        // Show success message
        toast.success('Snippet zaktualizowany pomyślnie!');

        // Close dialog
        onOpenChange(false);
      } catch (error) {
        console.error('Error updating snippet:', error);
        toast.error('Wystąpił błąd podczas aktualizacji snippetu');
      }
    }
  });

  // Update form and state when snippet changes
  useEffect(() => {
    if (snippet) {
      form.reset({
        title: snippet.title,
        description: snippet.description ?? '',
        language: snippet.language,
        content: snippet.content
      });
      setTags(snippet.tags ?? []);
      setSelectedCollections(snippet.collections.map((c) => ({ id: c.id, name: c.name })) ?? []);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [snippet]);

  const handleCancel = () => {
    onOpenChange(false);
  };

  if (isLoadingSnippet) {
    return (
      <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogContent className="max-w-[calc(100%-1rem)] p-3 sm:max-w-lg sm:p-6">
          <DialogHeader>
            <DialogTitle>Edytuj snippet</DialogTitle>
            <DialogDescription>Ładowanie danych snippetu...</DialogDescription>
          </DialogHeader>
          <div className="flex items-center justify-center p-8">
            <Spinner />
          </div>
        </DialogContent>
      </Dialog>
    );
  }

  if (!snippet) {
    return null;
  }

  return (
    <Dialog key={snippetId} open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[90vh] max-w-[calc(100%-1rem)] overflow-y-auto p-3 sm:max-w-[50vw] sm:p-6">
        <DialogHeader>
          <DialogTitle>Edytuj snippet</DialogTitle>
          <DialogDescription>Zaktualizuj informacje o fragmencie kodu</DialogDescription>
        </DialogHeader>

        <form
          id="edit-snippet-form"
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
                      className="font-mono text-sm h-64 resize-none"
                    />
                    {isInvalid && <FieldError errors={field.state.meta.errors} />}
                  </Field>
                );
              }}
            </form.Field>
          </FieldGroup>

          <div className="mt-4">
            <TagSelector selectedTags={tags} onTagsChange={setTags} />
          </div>

          <div className="mt-4">
            <CollectionSelector
              selectedCollections={selectedCollections}
              onCollectionsChange={setSelectedCollections}
            />
          </div>
        </form>

        <form.Subscribe selector={(state) => state.isSubmitting}>
          {(isSubmitting) => (
            <div className="flex justify-end gap-2 pt-4">
              <Button type="button" variant="outline" onClick={handleCancel} disabled={isSubmitting}>
                Anuluj
              </Button>
              <Button type="submit" form="edit-snippet-form" disabled={isSubmitting}>
                {isSubmitting ? (
                  <>
                    <Spinner className="mr-2" />
                    Zapisywanie...
                  </>
                ) : (
                  'Zapisz zmiany'
                )}
              </Button>
            </div>
          )}
        </form.Subscribe>
      </DialogContent>
    </Dialog>
  );
}
