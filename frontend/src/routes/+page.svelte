<script lang="ts">
	import { createGetUserSnippets } from '$lib/api/endpoints/snippets';
	import Header from '$lib/components/Header.svelte';
	import SnippetCard from '$lib/components/SnippetCard.svelte';

	/**
	 * Główna strona z listą wszystkich snippetów
	 */
	const snippetsQuery = createGetUserSnippets();

	// Stan wyszukiwania
	let searchQuery = $state('');

	// Filtrowane snippety
	const filteredSnippets = $derived(
		snippetsQuery.data?.filter((snippet: any) =>
			snippet.title.toLowerCase().includes(searchQuery.toLowerCase())
		) || []
	);
</script>

<div class="flex flex-col h-screen">
	<!-- Header z wyszukiwaniem -->
	<Header bind:onSearch={searchQuery} />

	<!-- Lista snippetów -->
	<div class="flex-1 overflow-auto p-8">
		{#if snippetsQuery.isLoading}
			<div class="text-center py-12">
				<p class="text-gray-600">Ładowanie snippetów...</p>
			</div>
		{:else if snippetsQuery.error}
			<div class="text-center py-12">
				<p class="text-red-500">Błąd podczas ładowania snippetów</p>
			</div>
		{:else if filteredSnippets.length === 0}
			<div class="text-center py-12">
				<p class="text-gray-600">Brak snippetów do wyświetlenia</p>
			</div>
		{:else}
			<div class="grid gap-4 max-w-4xl mx-auto">
				{#each filteredSnippets as snippet (snippet.id)}
					<SnippetCard {snippet} />
				{/each}
			</div>
		{/if}
	</div>
</div>
