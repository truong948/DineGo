import React, { useState } from 'react';
import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { 
  LayoutGrid, 
  UtensilsCrossed, 
  ClipboardList, 
  Receipt, 
  LogOut, 
  User, 
  ChefHat, 
  Menu as MenuIcon, 
  X,
  Bell
} from 'lucide-react';
import { useAuth } from '../../context/AuthContext';

const AdminLayout = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [sidebarOpen, setSidebarOpen] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const navItems = [
    {
      path: '/admin/tables',
      name: 'Sơ đồ bàn ăn',
      icon: LayoutGrid,
      badge: 'Chính'
    },
    {
      path: '/admin/menu',
      name: 'Quản lý thực đơn',
      icon: UtensilsCrossed,
    },
    {
      path: '/admin/orders',
      name: 'Phiếu Order & Bếp',
      icon: ClipboardList,
      count: 3
    },
    {
      path: '/admin/bills',
      name: 'Thanh toán & Hóa đơn',
      icon: Receipt,
    },
  ];

  return (
    <div className="min-h-screen bg-slate-950 flex flex-col md:flex-row font-sans text-slate-100">
      {/* Mobile Overlay */}
      {sidebarOpen && (
        <div 
          className="fixed inset-0 bg-slate-950/80 backdrop-blur-sm z-40 md:hidden"
          onClick={() => setSidebarOpen(false)}
        />
      )}

      {/* Sidebar Left */}
      <aside className={`
        fixed md:static inset-y-0 left-0 z-50
        w-64 bg-slate-900/90 backdrop-blur-md border-r border-slate-800/80
        flex flex-col justify-between transition-transform duration-300 ease-in-out
        ${sidebarOpen ? 'translate-x-0' : '-translate-x-full md:translate-x-0'}
      `}>
        <div>
          {/* Logo Header */}
          <div className="h-16 px-6 flex items-center justify-between border-b border-slate-800/80">
            <div className="flex items-center gap-3">
              <div className="w-9 h-9 rounded-xl bg-gradient-to-tr from-orange-600 to-amber-500 flex items-center justify-center shadow-lg shadow-orange-500/20">
                <ChefHat className="w-5 h-5 text-white" />
              </div>
              <div>
                <h1 className="font-bold text-lg text-white tracking-wide leading-none">
                  Dine<span className="text-orange-500">Go</span>
                </h1>
                <span className="text-[10px] text-slate-400 font-medium">REST-POS v1.0</span>
              </div>
            </div>
            <button 
              className="md:hidden text-slate-400 hover:text-white"
              onClick={() => setSidebarOpen(false)}
            >
              <X className="w-5 h-5" />
            </button>
          </div>

          {/* Navigation Items */}
          <nav className="p-4 space-y-1.5">
            <p className="px-3 text-[11px] font-semibold text-slate-500 uppercase tracking-wider mb-2">
              Quản lý nhà hàng
            </p>
            {navItems.map((item) => {
              const Icon = item.icon;
              return (
                <NavLink
                  key={item.path}
                  to={item.path}
                  onClick={() => setSidebarOpen(false)}
                  className={({ isActive }) => `
                    flex items-center justify-between px-3.5 py-3 rounded-xl text-sm font-medium transition-all duration-200
                    ${isActive 
                      ? 'bg-gradient-to-r from-orange-600 to-orange-500 text-white shadow-lg shadow-orange-500/20 font-semibold' 
                      : 'text-slate-400 hover:text-slate-200 hover:bg-slate-800/60'}
                  `}
                >
                  <div className="flex items-center gap-3">
                    <Icon className="w-5 h-5" />
                    <span>{item.name}</span>
                  </div>
                  {item.count && (
                    <span className="px-2 py-0.5 text-xs font-bold bg-orange-500/20 text-orange-400 rounded-full border border-orange-500/30">
                      {item.count}
                    </span>
                  )}
                  {item.badge && (
                    <span className="px-2 py-0.5 text-[10px] font-semibold bg-emerald-500/20 text-emerald-400 rounded-md border border-emerald-500/30">
                      {item.badge}
                    </span>
                  )}
                </NavLink>
              );
            })}
          </nav>
        </div>

        {/* Sidebar Footer - Quick System Status */}
        <div className="p-4 border-t border-slate-800/80">
          <div className="p-3 rounded-xl bg-slate-800/40 border border-slate-800 flex items-center justify-between">
            <div className="flex items-center gap-2">
              <span className="relative flex h-2.5 w-2.5">
                <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-emerald-400 opacity-75"></span>
                <span className="relative inline-flex rounded-full h-2.5 w-2.5 bg-emerald-500"></span>
              </span>
              <span className="text-xs text-slate-300 font-medium">API .NET 8 Online</span>
            </div>
            <span className="text-[10px] text-slate-500">Port 7123</span>
          </div>
        </div>
      </aside>

      {/* Main Content Area */}
      <div className="flex-1 flex flex-col min-w-0">
        {/* Top Header */}
        <header className="h-16 bg-slate-900/60 backdrop-blur-md border-b border-slate-800/80 px-4 md:px-8 flex items-center justify-between sticky top-0 z-30">
          {/* Mobile menu trigger */}
          <div className="flex items-center gap-3">
            <button 
              onClick={() => setSidebarOpen(true)}
              className="p-2 rounded-lg bg-slate-800 text-slate-300 hover:text-white md:hidden"
            >
              <MenuIcon className="w-5 h-5" />
            </button>
            <h2 className="text-base md:text-lg font-semibold text-slate-200 hidden sm:block">
              Hệ thống POS Quản lý DineGo
            </h2>
          </div>

          {/* Header Right - User info & logout */}
          <div className="flex items-center gap-4">
            {/* Notification button */}
            <button className="relative p-2 rounded-xl text-slate-400 hover:text-slate-200 hover:bg-slate-800/60 transition-colors">
              <Bell className="w-5 h-5" />
              <span className="absolute top-1.5 right-1.5 w-2 h-2 rounded-full bg-orange-500"></span>
            </button>

            <div className="h-6 w-px bg-slate-800"></div>

            {/* Staff Info */}
            <div className="flex items-center gap-3">
              <div className="w-9 h-9 rounded-full bg-gradient-to-tr from-slate-700 to-slate-600 border border-slate-500/30 flex items-center justify-center text-orange-400 font-bold shadow-sm">
                {user?.fullName ? user.fullName.charAt(0).toUpperCase() : <User className="w-5 h-5" />}
              </div>
              <div className="hidden sm:block text-left">
                <p className="text-sm font-semibold text-slate-200 leading-tight">
                  {user?.fullName || 'Nguyễn Văn Quản Lý'}
                </p>
                <p className="text-xs text-orange-400 font-medium">
                  {user?.role || 'Quản lý ca / Thu ngân'}
                </p>
              </div>
            </div>

            {/* Logout Button */}
            <button
              onClick={handleLogout}
              title="Đăng xuất khỏi hệ thống"
              className="flex items-center gap-2 px-3 py-1.5 rounded-xl bg-red-500/10 text-red-400 hover:bg-red-500 hover:text-white border border-red-500/20 text-xs font-semibold transition-all duration-200"
            >
              <LogOut className="w-4 h-4" />
              <span className="hidden md:inline">Đăng xuất</span>
            </button>
          </div>
        </header>

        {/* Dynamic Page Content */}
        <main className="flex-1 p-4 md:p-6 overflow-y-auto bg-slate-950">
          <Outlet />
        </main>
      </div>
    </div>
  );
};

export default AdminLayout;
