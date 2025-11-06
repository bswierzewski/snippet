import { createClient } from '@/lib/supabase/client';
import Axios, { AxiosRequestConfig } from 'axios';

export const AXIOS_INSTANCE = Axios.create({
  baseURL: 'http://localhost:7000'
});

// Create Supabase client once (singleton) - shared across all requests
const supabase = createClient();

// Interceptor: Automatically attach JWT token to every API request
// Backend uses this token to identify the user and authorize requests
AXIOS_INSTANCE.interceptors.request.use(async (config) => {
  const {
    data: { session }
  } = await supabase.auth.getSession();

  if (session?.access_token) {
    config.headers.Authorization = `Bearer ${session.access_token}`;
  }

  return config;
});

// Custom instance for Orval-generated API client
// Adds cancellation support for React Query
export const customInstance = <T>(config: AxiosRequestConfig): Promise<T> => {
  const source = Axios.CancelToken.source();
  const promise = AXIOS_INSTANCE({
    ...config,
    cancelToken: source.token
  }).then(({ data }) => data);

  // Attach cancel method to promise for React Query integration
  // @ts-ignore
  promise.cancel = () => {
    source.cancel('Query was cancelled');
  };

  return promise;
};
