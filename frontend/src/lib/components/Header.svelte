<script lang="ts">
	import UserButton from './UserButton.svelte';

	/**
	 * Komponent nagłówka z wyszukiwaniem, filtrami i akcjami
	 */
	let { onSearch = $bindable('') } = $props<{
		onSearch?: string;
	}>();
</script>

<header class="border-b border-gray-200 bg-white px-8 py-4">
	<div class="flex items-center justify-between gap-4">
		<!-- Wyszukiwarka -->
		<div class="flex-1 max-w-2xl">
			<input
				type="text"
				placeholder="Szukaj snippetów..."
				bind:value={onSearch}
				class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-600"
			/>
		</div>

		<!-- Filtry -->
		<div class="flex items-center gap-2">
			<select class="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none">
				<option>Filtruj po języku</option>
			</select>

			<select class="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none">
				<option>Filtruj po tagu</option>
			</select>

			<button class="px-4 py-2 text-gray-600 hover:text-gray-900">Wyczyść filtry</button>
		</div>

		<!-- Akcje -->
		<div class="flex items-center gap-2">
			<button
				class="px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 flex items-center gap-2"
			>
				<span>+</span>
				<span>Nowy snippet</span>
			</button>

			<!-- Dark mode toggle -->
			<button class="p-2 rounded-lg hover:bg-gray-100">🌙</button>

			<!-- User menu -->
			<UserButton />
		</div>
	</div>
</header>
