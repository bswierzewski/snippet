import { create } from 'zustand';

export interface FilterState {
  searchTerm: string;
  selectedTags: string[]; // Tag names
  selectedLanguages: number[]; // ProgrammingLanguage enum values
  selectedCollectionId: string | null;

  // Actions
  setSearchTerm: (term: string) => void;
  setSelectedTags: (tags: string[]) => void;
  setSelectedLanguages: (languages: number[]) => void;
  setSelectedCollectionId: (collectionId: string | null) => void;
  clearFilters: () => void;
  hasActiveFilters: () => boolean;
  getActiveFilterCount: () => number;
}

export const useFilterStore = create<FilterState>((set, get) => ({
  searchTerm: '',
  selectedTags: [],
  selectedLanguages: [],
  selectedCollectionId: null,

  setSearchTerm: (term: string) => set({ searchTerm: term }),

  setSelectedTags: (tags: string[]) => set({ selectedTags: tags }),

  setSelectedLanguages: (languages: number[]) => set({ selectedLanguages: languages }),

  setSelectedCollectionId: (collectionId: string | null) => set({ selectedCollectionId: collectionId }),

  clearFilters: () => set({
    searchTerm: '',
    selectedTags: [],
    selectedLanguages: [],
    selectedCollectionId: null
  }),

  hasActiveFilters: () => {
    const state = get();
    return (
      state.searchTerm.length > 0 ||
      state.selectedTags.length > 0 ||
      state.selectedLanguages.length > 0 ||
      state.selectedCollectionId !== null
    );
  },

  getActiveFilterCount: () => {
    const state = get();
    let count = 0;
    if (state.searchTerm.length > 0) count++;
    if (state.selectedTags.length > 0) count += state.selectedTags.length;
    if (state.selectedLanguages.length > 0) count += state.selectedLanguages.length;
    if (state.selectedCollectionId !== null) count++;
    return count;
  }
}));
