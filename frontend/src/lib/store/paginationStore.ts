import { create } from 'zustand';

export interface PaginationState {
  currentPage: number;
  pageSize: number;
  totalItems: number;
  hasMore: boolean;
  isFetching: boolean;

  // Actions
  setCurrentPage: (page: number) => void;
  incrementPage: () => void;
  resetPagination: () => void;
  setTotalItems: (total: number) => void;
  setHasMore: (hasMore: boolean) => void;
  setIsFetching: (isFetching: boolean) => void;
}

const PAGE_SIZE = 10;

export const usePaginationStore = create<PaginationState>((set, get) => ({
  currentPage: 1,
  pageSize: PAGE_SIZE,
  totalItems: 0,
  hasMore: true,
  isFetching: false,

  setCurrentPage: (page: number) => set({ currentPage: page }),

  incrementPage: () => {
    const state = get();
    set({ currentPage: state.currentPage + 1 });
  },

  resetPagination: () => {
    set({
      currentPage: 1,
      totalItems: 0,
      hasMore: true,
      isFetching: false
    });
  },

  setTotalItems: (total: number) => {
    const state = get();
    const hasMore = state.currentPage * state.pageSize < total;
    set({ totalItems: total, hasMore });
  },

  setHasMore: (hasMore: boolean) => set({ hasMore }),

  setIsFetching: (isFetching: boolean) => set({ isFetching })
}));
