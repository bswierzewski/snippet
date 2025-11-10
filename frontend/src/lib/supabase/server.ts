import { createServerClient } from '@supabase/ssr';
import { cookies } from 'next/headers';

/**
 * Creates Supabase client for Server Components
 *
 * IMPORTANT: This is NOT a singleton (unlike the browser client)
 * Each server request needs a fresh client with request-specific cookies
 *
 * Why? Each HTTP request can be from a different user with different session cookies.
 * Sharing a single client would mix sessions between users = major security issue.
 *
 * Next.js cookies() is request-scoped, so this is safe and necessary.
 */
export async function createClient() {
  const cookieStore = await cookies();

  const supabaseUrl = process.env.NEXT_PUBLIC_SUPABASE_URL;
  const supabaseAnonKey = process.env.NEXT_PUBLIC_SUPABASE_ANON_KEY;

  if (!supabaseUrl || !supabaseAnonKey) {
    throw new Error(
      'Missing Supabase environment variables. Please set NEXT_PUBLIC_SUPABASE_URL and NEXT_PUBLIC_SUPABASE_ANON_KEY'
    );
  }

  return createServerClient(supabaseUrl, supabaseAnonKey, {
    cookies: {
      getAll() {
        return cookieStore.getAll();
      },
      setAll(cookiesToSet) {
        try {
          cookiesToSet.forEach(({ name, value, options }) => cookieStore.set(name, value, options));
        } catch {
          // Silently fail in Server Components where cookies are read-only
        }
      }
    }
  });
}
