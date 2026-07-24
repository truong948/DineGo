import React from 'react';
import { Receipt, DollarSign, Download, Printer, CheckCircle, Calendar } from 'lucide-react';

const BillsPage = () => {
  const bills = [
    { id: 'INV-20260724-01', table: 'Bàn 02', total: 450000, method: 'Chuyển khoản QR', status: 'Đã thanh toán', time: '12:15' },
    { id: 'INV-20260724-02', table: 'VIP 01', total: 1850000, method: 'Thẻ Visa/Master', status: 'Đã thanh toán', time: '11:50' },
    { id: 'INV-20260724-03', table: 'Bàn 06', total: 620000, method: 'Tiền mặt', status: 'Đã thanh toán', time: '11:10' },
  ];

  return (
    <div className="space-y-6">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 bg-slate-900/60 p-5 rounded-2xl border border-slate-800 backdrop-blur-md">
        <div>
          <h2 className="text-xl font-bold text-slate-100 flex items-center gap-2">
            <Receipt className="w-6 h-6 text-orange-500" /> Thanh Toán & Lịch Sử Hóa Đơn
          </h2>
          <p className="text-xs text-slate-400 mt-1">
            Tổng hợp doanh thu ca trực và danh sách hóa đơn thanh toán của khách hàng
          </p>
        </div>
      </div>

      {/* Bill List Table */}
      <div className="bg-slate-900/60 border border-slate-800 rounded-2xl overflow-hidden shadow-xl">
        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-slate-800/60 text-slate-400 text-[11px] uppercase tracking-wider font-semibold border-b border-slate-800">
                <th className="p-4">Mã Hóa Đơn</th>
                <th className="p-4">Bàn Ăn</th>
                <th className="p-4">Giờ Thanh Toán</th>
                <th className="p-4">Hình Thức</th>
                <th className="p-4">Tổng Tiền</th>
                <th className="p-4">Trạng Thái</th>
                <th className="p-4 text-right">Thao Tác</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-800 text-xs text-slate-200">
              {bills.map((bill) => (
                <tr key={bill.id} className="hover:bg-slate-800/40 transition-colors">
                  <td className="p-4 font-bold text-orange-400">{bill.id}</td>
                  <td className="p-4 font-semibold">{bill.table}</td>
                  <td className="p-4 text-slate-400">{bill.time}</td>
                  <td className="p-4">{bill.method}</td>
                  <td className="p-4 font-extrabold text-white">{bill.total.toLocaleString('vi-VN')} đ</td>
                  <td className="p-4">
                    <span className="px-2.5 py-1 rounded-full text-[10px] font-bold bg-emerald-500/20 text-emerald-400 border border-emerald-500/30">
                      {bill.status}
                    </span>
                  </td>
                  <td className="p-4 text-right">
                    <button 
                      onClick={() => alert(`In hóa đơn ${bill.id}`)}
                      className="p-1.5 rounded-lg bg-slate-800 hover:bg-slate-700 text-slate-300 hover:text-white transition-colors"
                    >
                      <Printer className="w-4 h-4" />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default BillsPage;
