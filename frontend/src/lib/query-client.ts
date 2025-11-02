import { QueryClient } from '@tanstack/svelte-query';

/**
 * QueryClient to główny obiekt TanStack Query.
 *
 * Konfiguracja:
 * - defaultOptions: domyślne ustawienia dla wszystkich query
 * - queries.staleTime: czas po którym dane są uznawane za "stare" (5 minut)
 * - queries.retry: ile razy ponawiać nieudane zapytanie (1 raz)
 */
export const queryClient = new QueryClient({
	defaultOptions: {
		queries: {
			staleTime: 1000 * 60 * 5, // 5 minut
			retry: 1
		}
	}
});
