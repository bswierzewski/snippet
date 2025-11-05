import Axios, { type AxiosRequestConfig } from 'axios';
import { getAccessToken } from '$lib/supabase';

export const AXIOS_INSTANCE = Axios.create({
	baseURL: 'http://localhost:7000'
});

// Interceptor dodający JWT token do każdego requesta
AXIOS_INSTANCE.interceptors.request.use(
	async (config) => {
		try {
			const token = await getAccessToken();
			if (token) {
				config.headers.Authorization = `Bearer ${token}`;
			}
		} catch (error) {
			console.error('Failed to get Supabase token:', error);
		}
		return config;
	},
	(error) => {
		return Promise.reject(error);
	}
);

export const customInstance = <T>(config: AxiosRequestConfig): Promise<T> => {
	const source = Axios.CancelToken.source();
	const promise = AXIOS_INSTANCE({
		...config,
		cancelToken: source.token
	}).then(({ data }) => data);

	// @ts-expect-error - adding cancel to promise
	promise.cancel = () => {
		source.cancel('Query was cancelled');
	};

	return promise;
};

export default customInstance;
