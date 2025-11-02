<script lang="ts">
	import { createGetCollections } from '$lib/api/endpoints/collections/collections';

	/**
	 * Komponent lewego panelu z kolekcjami
	 */
	const collectionsQuery = createGetCollections();
</script>

<aside class="w-64 h-screen border-r border-gray-200 bg-white p-4">
	<!-- Logo -->
	<div class="flex items-center gap-2 mb-8">
		<div class="w-8 h-8 bg-purple-600 rounded-lg"></div>
		<h1 class="text-xl font-bold">SnippetVault</h1>
	</div>

	<!-- Sekcja Kolekcje -->
	<div>
		<div class="flex items-center justify-between mb-2">
			<h2 class="text-sm font-semibold text-gray-600 uppercase">Kolekcje</h2>
			<button class="text-gray-600 hover:text-gray-900">+</button>
		</div>

		<nav class="space-y-1">
			<!-- Wszystkie snippety -->
			<a
				href="/"
				class="flex items-center gap-2 px-3 py-2 rounded-lg hover:bg-gray-100 text-gray-700"
			>
				<span>📁</span>
				<span>Wszystkie snippety</span>
			</a>

			<!-- Lista kolekcji -->
			{#if $collectionsQuery.isLoading}
				<p class="text-sm text-gray-500 px-3">Ładowanie...</p>
			{:else if $collectionsQuery.data}
				{#each $collectionsQuery.data as collection}
					<a
						href="/collections/{collection.id}"
						class="flex items-center justify-between px-3 py-2 rounded-lg hover:bg-gray-100 text-gray-700"
					>
						<div class="flex items-center gap-2">
							<span>📁</span>
							<span>{collection.name}</span>
						</div>
						<span class="text-xs text-gray-500">{collection.snippetsCount}</span>
					</a>
				{/each}
			{/if}
		</nav>
	</div>
</aside>
