'use client';

import { createClient } from '@/lib/supabase/client';
import { useForm } from '@tanstack/react-form';
import { useRouter } from 'next/navigation';
import { toast } from 'sonner';
import { z } from 'zod';

import { Button } from '@/components/ui/button';
import { Field, FieldError, FieldGroup, FieldLabel } from '@/components/ui/field';
import { Input } from '@/components/ui/input';
import { Spinner } from '@/components/ui/spinner';

const loginSchema = z.object({
  email: z.email('Podaj prawidłowy adres email'),
  password: z.string().min(1, 'Hasło jest wymagane')
});

type LoginFormData = z.infer<typeof loginSchema>;

export default function LoginPage() {
  const router = useRouter();
  const supabase = createClient();

  const form = useForm({
    defaultValues: {
      email: '',
      password: ''
    } as LoginFormData,
    validators: {
      onSubmit: loginSchema
    },
    onSubmit: async ({ value }) => {
      const { error } = await supabase.auth.signInWithPassword(value);

      if (error) {
        toast.error(error.message);
      } else {
        toast.success('Zalogowano pomyślnie!');
        router.push('/');
      }
    }
  });

  return (
    <div className="min-h-screen bg-background flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        <div className="bg-card rounded-lg shadow-md border p-8">
          <h1 className="text-2xl font-bold text-foreground mb-6 text-center">Logowanie</h1>

          <form
            id="login-form"
            onSubmit={(e) => {
              e.preventDefault();
              form.handleSubmit();
            }}
          >
            <FieldGroup>
              <form.Field name="email">
                {(field) => {
                  const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
                  return (
                    <Field data-invalid={isInvalid}>
                      <FieldLabel htmlFor="email">Email</FieldLabel>
                      <Input
                        id="email"
                        name="email"
                        type="email"
                        placeholder="twoj@email.com"
                        value={field.state.value}
                        onChange={(e) => field.handleChange(e.target.value)}
                        onBlur={field.handleBlur}
                        aria-invalid={isInvalid}
                        autoComplete="email"
                      />
                      {isInvalid && <FieldError errors={field.state.meta.errors} />}
                    </Field>
                  );
                }}
              </form.Field>

              <form.Field name="password">
                {(field) => {
                  const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
                  return (
                    <Field data-invalid={isInvalid}>
                      <FieldLabel htmlFor="password">Hasło</FieldLabel>
                      <Input
                        id="password"
                        name="password"
                        type="password"
                        placeholder="••••••••"
                        value={field.state.value}
                        onChange={(e) => field.handleChange(e.target.value)}
                        onBlur={field.handleBlur}
                        aria-invalid={isInvalid}
                        autoComplete="current-password"
                      />
                      {isInvalid && <FieldError errors={field.state.meta.errors} />}
                    </Field>
                  );
                }}
              </form.Field>
            </FieldGroup>
          </form>

          <form.Subscribe selector={(state) => state.isSubmitting}>
            {(isSubmitting) => (
              <Button type="submit" form="login-form" className="w-full mt-6" disabled={isSubmitting}>
                {isSubmitting ? (
                  <>
                    <Spinner className="mr-2" />
                    Logowanie...
                  </>
                ) : (
                  'Zaloguj się'
                )}
              </Button>
            )}
          </form.Subscribe>

          <p className="mt-4 text-center text-sm text-muted-foreground">
            Nie masz konta?{' '}
            <a href="/register" className="text-primary hover:text-primary/80 font-medium transition-colors">
              Zarejestruj się
            </a>
          </p>
        </div>
      </div>
    </div>
  );
}
