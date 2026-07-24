import React, { useState } from 'react';
import { ClipboardList, ChefHat, CheckCircle2, Clock, AlertTriangle, ArrowRight } from 'lucide-react';

const OrdersPage = () => {
  const initialOrders = [
    {
      id: 'ORD-101',
      tableName: 'Bàn 02',
      zone: 'Khu A',
      time: '11:30',
      status: 'Đang chế biến',
      items: [
        { name: 'Bò Mỹ Nướng Đá Yakiniku', qty: 1, note: 'Không hành' },
        { name: 'Bia Craft IPA 500ml', qty: 2, note: 'Uống kèm đá' },
      ],
    },
    {
      id: 'ORD-102',
      tableName: 'VIP 01',
      zone: 'Phòng VIP',
      time: '10:45',
      status: 'Chờ lên món',
      items: [
        { name: 'Lẩu Thái Hải Sản TomYum', qty: 1, note: 'Cay vừa' },
        { name: 'Salad Hoàng Gia Sốt Truffle', qty: 2, note: '' },
      ],
    },
    {
      id: 'ORD-103',
      tableName: 'Bàn 06',
      zone: 'Sân thượng',
      time: '11:50',
      status: 'Hoàn thành',
      items: [
        { name: 'Trà Trái Cây Nhiệt Đới', qty: 3, note: 'Ít đường' },
      ],
    },
  ];

  const [orders, setOrders] = useState(initialOrders);

  const handleNextStatus = (id) => {
    setOrders(prev => prev.map(o => {
      if (o.id === id) {
        const next = o.status === 'Đang chế biến' ? 'Chờ lên món' : 'Hoàn thành';
        return { ...o, status: next };
      }
      return o;
    }));
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 bg-slate-900/60 p-5 rounded-2xl border border-slate-800 backdrop-blur-md">
        <div>
          <h2 className="text-xl font-bold text-slate-100 flex items-center gap-2">
            <ClipboardList className="w-6 h-6 text-orange-500" /> Quản Lý Order & Bếp KDS
          </h2>
          <p className="text-xs text-slate-400 mt-1">
            Màn hình điều phối chế biến món ăn và trạng thái trả món ca trực
          </p>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        {orders.map((order) => (
          <div key={order.id} className="bg-slate-900/60 border border-slate-800 rounded-2xl p-5 space-y-4 flex flex-col justify-between">
            <div>
              <div className="flex items-center justify-between border-b border-slate-800 pb-3">
                <div>
                  <span className="text-[10px] uppercase font-bold text-orange-400">{order.zone}</span>
                  <h3 className="text-lg font-bold text-white">{order.tableName}</h3>
                </div>
                <span className="px-2.5 py-1 rounded-full text-xs font-bold bg-slate-800 text-slate-300 border border-slate-700">
                  {order.id}
                </span>
              </div>

              <div className="flex items-center gap-2 text-xs text-slate-400 my-3">
                <Clock className="w-3.5 h-3.5 text-orange-400" />
                <span>Thời gian tạo: {order.time}</span>
              </div>

              <div className="space-y-2">
                <p className="text-xs font-semibold text-slate-300">Danh sách món:</p>
                {order.items.map((item, idx) => (
                  <div key={idx} className="bg-slate-800/50 p-2.5 rounded-xl flex justify-between items-center text-xs">
                    <div>
                      <p className="font-semibold text-slate-200">{item.qty}x {item.name}</p>
                      {item.note && <span className="text-[10px] text-amber-400 italic">Ghi chú: {item.note}</span>}
                    </div>
                  </div>
                ))}
              </div>
            </div>

            <div className="pt-3 border-t border-slate-800 flex items-center justify-between">
              <span className={`px-2.5 py-1 rounded-md text-xs font-bold ${
                order.status === 'Hoàn thành' 
                  ? 'bg-emerald-500/20 text-emerald-400' 
                  : order.status === 'Chờ lên món' 
                  ? 'bg-amber-500/20 text-amber-400' 
                  : 'bg-rose-500/20 text-rose-400'
              }`}>
                {order.status}
              </span>

              {order.status !== 'Hoàn thành' && (
                <button
                  onClick={() => handleNextStatus(order.id)}
                  className="flex items-center gap-1 px-3 py-1.5 rounded-xl bg-orange-600 hover:bg-orange-500 text-white font-bold text-xs shadow-md transition-all"
                >
                  <span>Chuyển bước</span> <ArrowRight className="w-3.5 h-3.5" />
                </button>
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default OrdersPage;
