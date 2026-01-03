import { defineConfig } from 'orval';

export default defineConfig({
  snippetApi: {
    input: 'http://localhost:7000/openapi/v1.json',
    output: {
      target: './src/lib/api/endpoints',
      schemas: './src/lib/api/models',
      client: 'react-query',
      prettier: true,
      mode: 'tags',
      clean: true,
      override: {
        mutator: {
          path: './src/lib/api/axios-instance.ts',
          name: 'customInstance'
        },
        operations: {
          SearchSnippets: {
            query: {
              useInfinite: true,
              useInfiniteQueryParam: 'pageNumber'
            }
          }
        }
      }
    }
  }
});
