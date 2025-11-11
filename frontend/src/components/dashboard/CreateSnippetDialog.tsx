'use client';

import { useForm } from '@tanstack/react-form';
import { useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import { toast } from 'sonner';
import { z } from 'zod';

import { getGetUserCollectionsQueryKey } from '@/lib/api/endpoints/collections';
import { useGetProgrammingLanguageEnumValues } from '@/lib/api/endpoints/lookup-data';
import { getGetUserSnippetsQueryKey, useCreateSnippet } from '@/lib/api/endpoints/snippets';
import type { ProgrammingLanguage } from '@/lib/api/models';

import { CollectionSelector, type CollectionItem } from '@/components/dashboard/CollectionSelector';
import { TagSelector, type TagItem } from '@/components/dashboard/TagSelector';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog';
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
  const [tags, setTags] = useState<TagItem[]>([]);
  const [selectedCollections, setSelectedCollections] = useState<CollectionItem[]>([]);

  const queryClient = useQueryClient();
  const { data: programmingLanguages, isLoading: isLoadingLanguages } = useGetProgrammingLanguageEnumValues();
  const { mutateAsync: createSnippet } = useCreateSnippet();

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
        // Create the snippet with tags included
        await createSnippet({
          data: {
            title: value.title,
            description: value.description || null,
            language: value.language as ProgrammingLanguage,
            content: value.content,
            tagIds: tags.length > 0 ? tags.map((t) => t.id) : null,
            collectionIds: selectedCollections.length > 0 ? selectedCollections.map((c) => c.id) : null
          }
        });

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

  const resetForm = () => {
    form.reset();
    setTags([]);
    setSelectedCollections([]);
  };

  const handleCancel = () => {
    resetForm();
    onOpenChange(false);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[90vh] max-w-[calc(100%-1rem)] overflow-y-auto p-3 sm:max-w-[50vw] sm:p-6">
        <DialogHeader>
          <DialogTitle>Nowy snippet</DialogTitle>
          <DialogDescription>Utwórz nowy fragment kodu z tagami i przypisz go do kolekcji</DialogDescription>
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
