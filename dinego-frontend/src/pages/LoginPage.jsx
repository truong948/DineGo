import React, { useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { ChefHat, Lock, User, AlertCircle, ArrowRight, ShieldCheck } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import axiosClient from '../api/axiosClient';

const LoginPage = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const isExpired = searchParams.get('expired') === '1';

  const { login } = useAuth();
  const [username, setUsername] = useState('admin');
  const [password, setPassword] = useState('123456');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      // Gọi API đăng nhập tới .NET 8 Web API
      // const res = await axiosClient.post('/auth/login', { username, password });
      // login(res.token, res.user);

      // Demo login nếu backend chưa chạy
      setTimeout(() => {
        const mockToken = 'mock_jwt_token_' + Date.now();
        const mockUser = {
          id: 1,
          username: username,
          fullName: username === 'admin' ? 'Nguyễn Văn Quản Lý' : 'Trần Thị Thu Ngân',
          role: username === 'admin' ? 'Quản Lý Nhà Hàng' : 'Thu Ngân / Phục Vụ'
        };
        login(mockToken, mockUser);
        setLoading(false);
        navigate('/admin/tables');
      }, 700);

    } catch (err) {
      setLoading(false);
      setError(err.response?.data?.message || 'Tên đăng nhập hoặc mật khẩu không chính xác!');
    }
  };

  const handleQuickDemo = (role) => {
    if (role === 'admin') {
      setUsername('admin');
      setPassword('admin123');
    } else {
      setUsername('cashier');
      setPassword('cashier123');
    }
  };

  return (
    <div className="min-h-screen bg-slate-950 flex items-center justify-center p-4 relative overflow-hidden font-sans">
      {/* Dynamic Background Glow Effect */}
      <div className="absolute top-1/4 left-1/2 -translate-x-1/2 -translate-y-1/2 w-96 h-96 bg-orange-600/15 rounded-full blur-3xl pointer-events-none"></div>
      <div className="absolute bottom-1/4 right-1/4 w-80 h-80 bg-amber-600/10 rounded-full blur-3xl pointer-events-none"></div>

      <div className="w-full max-w-md bg-slate-900/80 backdrop-blur-xl border border-slate-800 rounded-3xl p-8 shadow-2xl relative z-10">
        {/* Header Logo */}
        <div className="flex flex-col items-center mb-8 text-center">
          <div className="w-14 h-14 rounded-2xl bg-gradient-to-tr from-orange-600 to-amber-500 flex items-center justify-center shadow-xl shadow-orange-500/20 mb-3">
            <ChefHat className="w-8 h-8 text-white" />
          </div>
          <h1 className="text-2xl font-extrabold text-white tracking-tight">
            Dine<span className="text-orange-500">Go</span> REST-POS
          </h1>
          <p className="text-xs text-slate-400 mt-1">
            Đăng nhập hệ thống quản lý nhà hàng & bàn ăn
          </p>
        </div>

        {/* Expired alert from Axios interceptor 401 redirect */}
        {isExpired && (
          <div className="mb-4 p-3 rounded-xl bg-amber-500/10 border border-amber-500/30 text-amber-400 text-xs font-medium flex items-center gap-2">
            <AlertCircle className="w-4 h-4 shrink-0" />
            <span>Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.</span>
          </div>
        )}

        {/* Error alert */}
        {error && (
          <div className="mb-4 p-3 rounded-xl bg-rose-500/10 border border-rose-500/30 text-rose-400 text-xs font-medium flex items-center gap-2">
            <AlertCircle className="w-4 h-4 shrink-0" />
            <span>{error}</span>
          </div>
        )}

        {/* Login Form */}
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="text-xs font-semibold text-slate-300 block mb-1.5">
              Tài khoản nhân viên
            </label>
            <div className="relative">
              <User className="w-4 h-4 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
              <input
                type="text"
                required
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                placeholder="Nhập username"
                className="w-full bg-slate-800/80 border border-slate-700/80 focus:border-orange-500 text-white text-sm rounded-xl pl-10 pr-4 py-3 outline-none transition-all placeholder:text-slate-500"
              />
            </div>
          </div>

          <div>
            <label className="text-xs font-semibold text-slate-300 block mb-1.5">
              Mật khẩu
            </label>
            <div className="relative">
              <Lock className="w-4 h-4 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
              <input
                type="password"
                required
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="••••••••"
                className="w-full bg-slate-800/80 border border-slate-700/80 focus:border-orange-500 text-white text-sm rounded-xl pl-10 pr-4 py-3 outline-none transition-all placeholder:text-slate-500"
              />
            </div>
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full py-3.5 rounded-xl bg-gradient-to-r from-orange-600 to-orange-500 hover:from-orange-500 hover:to-orange-400 text-white font-bold text-sm shadow-lg shadow-orange-500/25 transition-all flex items-center justify-center gap-2 group mt-2"
          >
            {loading ? (
              <span className="inline-block w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin"></span>
            ) : (
              <>
                <span>Đăng nhập ca làm việc</span>
                <ArrowRight className="w-4 h-4 group-hover:translate-x-1 transition-transform" />
              </>
            )}
          </button>
        </form>

        {/* Demo Fast Login Buttons */}
        <div className="mt-6 pt-5 border-t border-slate-800">
          <p className="text-[11px] text-slate-500 uppercase tracking-wider font-semibold text-center mb-2">
            Thử nghiệm nhanh ca làm việc
          </p>
          <div className="flex gap-2">
            <button
              onClick={() => handleQuickDemo('admin')}
              className="flex-1 py-2 px-3 rounded-xl bg-slate-800/60 hover:bg-slate-800 border border-slate-700 text-xs font-medium text-slate-300 transition-colors"
            >
              🔑 Quản lý (admin)
            </button>
            <button
              onClick={() => handleQuickDemo('cashier')}
              className="flex-1 py-2 px-3 rounded-xl bg-slate-800/60 hover:bg-slate-800 border border-slate-700 text-xs font-medium text-slate-300 transition-colors"
            >
              💵 Thu ngân (cashier)
            </button>
          </div>
        </div>

        {/* Footer info */}
        <div className="mt-6 text-center text-[11px] text-slate-500 flex items-center justify-center gap-1">
          <ShieldCheck className="w-3.5 h-3.5 text-emerald-500" />
          <span>Kết nối bảo mật tới Backend .NET 8 Web API</span>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
