import axios from 'axios';

// Định nghĩa baseURL cho .NET 8 Web API
// Hỗ trợ cả địa chỉ mặc định trong môi trường dev hoặc cấu hình thông qua VITE_API_URL
const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7123/api';

const axiosClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000,
});

// 1. Request Interceptor: Tự động đính kèm JWT Bearer Token vào mọi Request
axiosClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('dinego_token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// 2. Response Interceptor: Xử lý Response & tự động Đăng xuất khi token hết hạn / lỗi 401
axiosClient.interceptors.response.use(
  (response) => {
    // Trả về trực tiếp response.data để tối giản dữ liệu khi gọi API
    return response.data !== undefined ? response.data : response;
  },
  (error) => {
    if (error.response) {
      const { status } = error.response;
      
      // Nếu mã lỗi là 401 (Unauthorized) - Token không hợp lệ hoặc đã hết hạn
      if (status === 401) {
        console.warn('[Axios Interceptor] Unauthorized! Xóa token và chuyển hướng về /login...');
        localStorage.removeItem('dinego_token');
        localStorage.removeItem('dinego_user');
        
        // Điều hướng người dùng về trang Đăng nhập nếu chưa ở trang /login
        if (window.location.pathname !== '/login') {
          window.location.href = '/login?expired=1';
        }
      }
    }
    return Promise.reject(error);
  }
);

export default axiosClient;
