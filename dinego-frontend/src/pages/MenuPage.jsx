import React, { useState } from 'react';
import { UtensilsCrossed, Plus, Search, Filter, Edit, Trash2, Tag, Check, X } from 'lucide-react';

const MenuPage = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('All');

  const categories = ['All', 'Món Chính', 'Khai Vị', 'Nướng & Lẩu', 'Đồ Uống', 'Tráng Miệng'];

  const initialMenuItems = [
    { id: 1, name: 'Bò Mỹ Nướng Đá Yakiniku', category: 'Nướng & Lẩu', price: 320000, status: 'Đang bán', image: '🥩' },
    { id: 2, name: 'Lẩu Thái Hải Sản TomYum', category: 'Nướng & Lẩu', price: 450000, status: 'Đang bán', image: '🍲' },
    { id: 3, name: 'Salad Hoàng Gia Sốt Truffle', category: 'Khai Vị', price: 145000, status: 'Đang bán', image: '🥗' },
    { id: 4, name: 'Cánh Gà Chiên Mắm Tỏi', category: 'Khai Vị', price: 120000, status: 'Tạm hết', image: '🍗' },
    { id: 5, name: 'Bia Craft IPA 500ml', category: 'Đồ Uống', price: 65000, status: 'Đang bán', image: '🍺' },
    { id: 6, name: 'Trà Trái Cây Nhiệt Đới', category: 'Đồ Uống', price: 45000, status: 'Đang bán', image: '🍹' },
  ];

  const [menuItems, setMenuItems] = useState(initialMenuItems);

  const filteredItems = menuItems.filter(item => {
    const matchesCategory = selectedCategory === 'All' || item.category === selectedCategory;
    const matchesSearch = item.name.toLowerCase().includes(searchTerm.toLowerCase());
    return matchesCategory && matchesSearch;
  });

  return (
    <div className="space-y-6">
      {/* Top Header & Search */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 bg-slate-900/60 p-5 rounded-2xl border border-slate-800 backdrop-blur-md">
        <div>
          <h2 className="text-xl font-bold text-slate-100 flex items-center gap-2">
            <UtensilsCrossed className="w-6 h-6 text-orange-500" /> Quản Lý Thực Đơn & Món Ăn
          </h2>
          <p className="text-xs text-slate-400 mt-1">
            Cập nhật danh mục thực đơn, đơn giá và trạng thái còn món/hết món
          </p>
        </div>

        <button 
          onClick={() => alert('Chức năng thêm món mới')}
          className="flex items-center gap-2 px-4 py-2.5 rounded-xl bg-orange-600 hover:bg-orange-500 text-white font-bold text-xs shadow-lg shadow-orange-500/20 transition-all self-start md:self-auto"
        >
          <Plus className="w-4 h-4" /> Thêm Món Ăn Mới
        </button>
      </div>

      {/* Filter & Search Bar */}
      <div className="flex flex-col sm:flex-row gap-3">
        <div className="relative flex-1">
          <Search className="w-4 h-4 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
          <input
            type="text"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            placeholder="Tìm kiếm món ăn theo tên..."
            className="w-full bg-slate-900/80 border border-slate-800 rounded-xl pl-10 pr-4 py-2.5 text-xs text-white outline-none focus:border-orange-500 transition-colors"
          />
        </div>

        <div className="flex items-center gap-1.5 overflow-x-auto">
          {categories.map((cat) => (
            <button
              key={cat}
              onClick={() => setSelectedCategory(cat)}
              className={`px-3 py-2 rounded-xl text-xs font-semibold whitespace-nowrap transition-all ${
                selectedCategory === cat
                  ? 'bg-orange-500 text-white shadow-md'
                  : 'bg-slate-900/60 text-slate-400 border border-slate-800 hover:text-slate-200'
              }`}
            >
              {cat}
            </button>
          ))}
        </div>
      </div>

      {/* Menu Cards Grid */}
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
        {filteredItems.map((item) => (
          <div 
            key={item.id}
            className="bg-slate-900/60 border border-slate-800 hover:border-slate-700 rounded-2xl p-4 flex gap-4 items-center transition-all shadow-md"
          >
            <div className="w-16 h-16 rounded-xl bg-slate-800/80 border border-slate-700 flex items-center justify-center text-3xl shrink-0">
              {item.image}
            </div>

            <div className="flex-1 min-w-0">
              <span className="text-[10px] uppercase font-bold text-orange-400 tracking-wider block">
                {item.category}
              </span>
              <h4 className="font-bold text-slate-100 text-sm truncate">
                {item.name}
              </h4>
              <p className="text-sm font-extrabold text-white mt-1">
                {item.price.toLocaleString('vi-VN')} đ
              </p>
              
              <div className="flex items-center justify-between mt-2 pt-2 border-t border-slate-800/60 text-xs">
                <span className={`px-2 py-0.5 rounded-full text-[10px] font-bold ${
                  item.status === 'Đang bán' ? 'bg-emerald-500/20 text-emerald-400' : 'bg-slate-800 text-slate-400'
                }`}>
                  {item.status}
                </span>

                <div className="flex items-center gap-1">
                  <button className="p-1 rounded-lg text-slate-400 hover:text-white hover:bg-slate-800">
                    <Edit className="w-3.5 h-3.5" />
                  </button>
                  <button className="p-1 rounded-lg text-slate-400 hover:text-rose-400 hover:bg-slate-800">
                    <Trash2 className="w-3.5 h-3.5" />
                  </button>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default MenuPage;
