import { createBrowserClient } from '@supabase/ssr';

/**
 * Creates Supabase client for Client Components (browser)
 *
 * Note: No singleton pattern here - createBrowserClient handles caching internally
 * Safe to call multiple times, it will return the same instance automatically
 */
export function createClient() {
    return createBrowserClient(
        process.env.NEXT_PUBLIC_SUPABASE_URL!,
        process.env.NEXT_PUBLIC_SUPABASE_ANON_KEY!
    );
}
