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

const registerSchema = z
  .object({
    email: z.email('Podaj prawidłowy adres email'),
    password: z.string().min(8, 'Hasło musi mieć co najmniej 8 znaków'),
    confirmPassword: z.string().min(1, 'Potwierdź hasło')
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: 'Hasła muszą być takie same',
    path: ['confirmPassword']
  });

type RegisterFormData = z.infer<typeof registerSchema>;

export default function RegisterPage() {
  const router = useRouter();
  const supabase = createClient();

  const form = useForm({
    defaultValues: {
      email: '',
      password: '',
      confirmPassword: ''
    } as RegisterFormData,
    validators: {
      onSubmit: registerSchema
    },
    onSubmit: async ({ value }) => {
      const { error } = await supabase.auth.signUp({
        email: value.email,
        password: value.password
      });

      if (error) {
        toast.error(error.message);
      } else {
        toast.success('Konto zostało utworzone! Sprawdź email, aby potwierdzić rejestrację.');
        router.push('/login');
      }
    }
  });

  return (
    <div className="min-h-screen bg-background flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        <div className="bg-card rounded-lg shadow-md border p-8">
          <h1 className="text-2xl font-bold text-foreground mb-6 text-center">Rejestracja</h1>

          <form
            id="register-form"
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
                        autoComplete="new-password"
                      />
                      {isInvalid && <FieldError errors={field.state.meta.errors} />}
                    </Field>
                  );
                }}
              </form.Field>

              <form.Field name="confirmPassword">
                {(field) => {
                  const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
                  return (
                    <Field data-invalid={isInvalid}>
                      <FieldLabel htmlFor="confirmPassword">Potwierdź hasło</FieldLabel>
                      <Input
                        id="confirmPassword"
                        name="confirmPassword"
                        type="password"
                        placeholder="••••••••"
                        value={field.state.value}
                        onChange={(e) => field.handleChange(e.target.value)}
                        onBlur={field.handleBlur}
                        aria-invalid={isInvalid}
                        autoComplete="new-password"
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
              <Button type="submit" form="register-form" className="w-full mt-6" disabled={isSubmitting}>
                {isSubmitting ? (
                  <>
                    <Spinner className="mr-2" />
                    Rejestracja...
                  </>
                ) : (
                  'Zarejestruj się'
                )}
              </Button>
            )}
          </form.Subscribe>

          <p className="mt-4 text-center text-sm text-muted-foreground">
            Masz już konto?{' '}
            <a href="/login" className="text-primary hover:text-primary/80 font-medium transition-colors">
              Zaloguj się
            </a>
          </p>
        </div>
      </div>
    </div>
  );
}
