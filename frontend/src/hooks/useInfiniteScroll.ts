import { useEffect, useRef } from 'react';

interface UseInfiniteScrollOptions {
  onLoadMore: () => void;
  isLoading: boolean;
  hasMore: boolean;
  threshold?: number;
}

/**
 * Custom hook for infinite scroll using Intersection Observer API
 * Based on production-ready patterns for reliable scroll detection
 */
export function useInfiniteScroll({
  onLoadMore,
  isLoading,
  hasMore,
  threshold = 0.1
}: UseInfiniteScrollOptions) {
  const loaderRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    // Don't set up observer if we're already loading or no more items
    if (isLoading || !hasMore) return;

    const observer = new IntersectionObserver(
      (entries) => {
        const [entry] = entries;
        // Trigger load more when the loader element becomes visible
        if (entry.isIntersecting) {
          onLoadMore();
        }
      },
      {
        threshold, // Trigger when threshold % of the element is visible
        rootMargin: '100px' // Start loading 100px before the element is visible
      }
    );

    const currentLoaderRef = loaderRef.current;
    if (currentLoaderRef) {
      observer.observe(currentLoaderRef);
    }

    return () => {
      if (currentLoaderRef) {
        observer.unobserve(currentLoaderRef);
      }
    };
  }, [onLoadMore, isLoading, hasMore, threshold]);

  return loaderRef;
}
