import { createBrowserClient } from '@supabase/ssr';
import type { SupabaseClient } from '@supabase/supabase-js';

// Singleton instance to avoid creating multiple Supabase clients
// This is critical for performance as we call getSession() on every HTTP request
let client: SupabaseClient | null = null;

export function createClient() {
    if (client) {
        return client;
    }

    client = createBrowserClient(
        process.env.NEXT_PUBLIC_SUPABASE_URL!,
        process.env.NEXT_PUBLIC_SUPABASE_ANON_KEY!
    );

    return client;
}
