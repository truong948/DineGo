import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import ProtectedRoute from './ProtectedRoute';
import AdminLayout from '../components/Layout/AdminLayout';

// Import Pages
import LoginPage from '../pages/LoginPage';
import TablesPage from '../pages/TablesPage';
import MenuPage from '../pages/MenuPage';
import OrdersPage from '../pages/OrdersPage';
import BillsPage from '../pages/BillsPage';

const AppRoutes = () => {
  return (
    <Routes>
      {/* 1. Public Route */}
      <Route path="/login" element={<LoginPage />} />

      {/* 2. Protected Admin Routes */}
      <Route element={<ProtectedRoute />}>
        <Route element={<AdminLayout />}>
          <Route path="/admin/tables" element={<TablesPage />} />
          <Route path="/admin/menu" element={<MenuPage />} />
          <Route path="/admin/orders" element={<OrdersPage />} />
          <Route path="/admin/bills" element={<BillsPage />} />
        </Route>
      </Route>

      {/* 3. Fallback redirects */}
      <Route path="/" element={<Navigate to="/admin/tables" replace />} />
      <Route path="*" element={<Navigate to="/admin/tables" replace />} />
    </Routes>
  );
};

export default AppRoutes;
