import React, { useState, useEffect } from 'react';
import { 
  DndContext, 
  useDraggable, 
  PointerSensor, 
  useSensor, 
  useSensors 
} from '@dnd-kit/core';
import { 
  Users, 
  CheckCircle2, 
  Clock, 
  AlertCircle, 
  Sparkles, 
  Save, 
  Plus, 
  Eye, 
  Grid, 
  Move, 
  RefreshCw,
  X,
  CreditCard,
  Utensils,
  Layers,
  ArrowRightLeft
} from 'lucide-react';
import axiosClient from '../api/axiosClient';

// Cấu hình mã màu & Nhãn trạng thái bàn ăn
// 0: Trống (Green), 1: Đã đặt (Yellow), 2: Đang có khách (Red), 3: Đang dọn (Gray)
export const TABLE_STATUS = {
  0: { label: 'Trống', color: 'bg-emerald-500', text: 'text-emerald-400', border: 'border-emerald-500/40', bgLight: 'bg-emerald-500/10' },
  1: { label: 'Đã đặt', color: 'bg-amber-500', text: 'text-amber-400', border: 'border-amber-500/40', bgLight: 'bg-amber-500/10' },
  2: { label: 'Đang có khách', color: 'bg-rose-500', text: 'text-rose-400', border: 'border-rose-500/40', bgLight: 'bg-rose-500/10' },
  3: { label: 'Đang dọn', color: 'bg-slate-500', text: 'text-slate-400', border: 'border-slate-500/40', bgLight: 'bg-slate-500/10' },
};

// Component từng Bàn ăn có khả năng Kéo thả (Draggable Table Card)
const DraggableTableCard = ({ table, isEditMode, onClick }) => {
  const { attributes, listeners, setNodeRef, transform, isDragging } = useDraggable({
    id: `table-${table.id}`,
    disabled: !isEditMode, // Chỉ cho kéo thả khi ở chế độ Chỉnh sửa sơ đồ
    data: { table },
  });

  const statusInfo = TABLE_STATUS[table.status] || TABLE_STATUS[0];

  // Tính toán vị trí X, Y (px) cộng dồn với khoảng cách transform khi đang drag
  const currentX = (table.positionX || 0) + (transform ? transform.x : 0);
  const currentY = (table.positionY || 0) + (transform ? transform.y : 0);

  const style = {
    position: 'absolute',
    left: `${currentX}px`,
    top: `${currentY}px`,
    touchAction: 'none',
  };

  return (
    <div
      ref={setNodeRef}
      style={style}
      {...(isEditMode ? { ...listeners, ...attributes } : {})}
      onClick={() => !isDragging && onClick(table)}
      className={`
        w-44 h-36 rounded-2xl p-3.5 flex flex-col justify-between cursor-pointer
        transition-all duration-150 select-none backdrop-blur-md border shadow-lg
        ${statusInfo.bgLight} ${statusInfo.border}
        ${isDragging ? 'opacity-80 scale-105 z-50 ring-2 ring-orange-500 shadow-2xl' : 'hover:scale-102 hover:shadow-xl'}
        ${isEditMode ? 'cursor-grab active:cursor-grabbing border-dashed border-orange-400' : ''}
      `}
    >
      {/* Header card: Tên bàn & Badge Trạng thái */}
      <div className="flex items-start justify-between">
        <div>
          <span className="text-[10px] uppercase tracking-wider font-semibold text-slate-400 block">
            {table.zoneName || 'Tầng 1'}
          </span>
          <h4 className="font-bold text-slate-100 text-base leading-tight">
            {table.name}
          </h4>
        </div>
        <span className={`px-2 py-0.5 rounded-full text-[10px] font-bold text-white shadow-sm ${statusInfo.color}`}>
          {statusInfo.label}
        </span>
      </div>

      {/* Thông tin số ghế & Giờ vào (nếu có khách) */}
      <div className="space-y-1 my-1">
        <div className="flex items-center gap-1.5 text-xs text-slate-300">
          <Users className="w-3.5 h-3.5 text-slate-400" />
          <span>{table.capacity} ghế</span>
        </div>
        {table.status === 2 && table.checkInTime && (
          <div className="flex items-center gap-1.5 text-[11px] text-rose-300 font-medium">
            <Clock className="w-3.5 h-3.5 animate-pulse" />
            <span>Vào ca: {table.checkInTime}</span>
          </div>
        )}
        {table.status === 2 && table.totalAmount && (
          <p className="text-xs font-bold text-rose-400">
            {table.totalAmount.toLocaleString('vi-VN')} đ
          </p>
        )}
      </div>

      {/* Footer card */}
      <div className="flex items-center justify-between pt-1 border-t border-slate-700/40 text-[11px]">
        {isEditMode ? (
          <span className="flex items-center gap-1 text-orange-400 font-medium">
            <Move className="w-3 h-3" /> Kéo để di chuyển
          </span>
        ) : (
          <span className="text-slate-400 hover:text-white transition-colors">
            Nhấn để xem chi tiết →
          </span>
        )}
      </div>
    </div>
  );
};

// Trang chính Sơ đồ bàn ăn (TablesPage)
const TablesPage = () => {
  // Mock dữ liệu bàn ăn ban đầu
  const initialTables = [
    { id: 1, name: 'Bàn 01', capacity: 4, status: 0, zoneName: 'Khu A - Tầng 1', positionX: 40, positionY: 40 },
    { id: 2, name: 'Bàn 02', capacity: 4, status: 2, zoneName: 'Khu A - Tầng 1', positionX: 240, positionY: 40, checkInTime: '11:30', totalAmount: 450000 },
    { id: 3, name: 'Bàn 03', capacity: 2, status: 1, zoneName: 'Khu A - Tầng 1', positionX: 440, positionY: 40, checkInTime: '12:00' },
    { id: 4, name: 'Bàn 04', capacity: 6, status: 0, zoneName: 'Khu A - Tầng 1', positionX: 640, positionY: 40 },
    { id: 5, name: 'VIP 01', capacity: 8, status: 2, zoneName: 'Phòng VIP', positionX: 40, positionY: 220, checkInTime: '10:45', totalAmount: 1850000 },
    { id: 6, name: 'VIP 02', capacity: 10, status: 3, zoneName: 'Phòng VIP', positionX: 240, positionY: 220 },
    { id: 7, name: 'Bàn 05', capacity: 4, status: 0, zoneName: 'Sân thượng', positionX: 440, positionY: 220 },
    { id: 8, name: 'Bàn 06', capacity: 4, status: 2, zoneName: 'Sân thượng', positionX: 640, positionY: 220, checkInTime: '11:50', totalAmount: 620000 },
  ];

  const [tables, setTables] = useState(initialTables);
  const [selectedZone, setSelectedZone] = useState('All');
  const [selectedTable, setSelectedTable] = useState(null);
  const [isEditMode, setIsEditMode] = useState(false);
  const [viewMode, setViewMode] = useState('canvas'); // 'canvas' | 'grid'
  const [savingLayout, setSavingLayout] = useState(false);

  // Cấu hình Sensor kéo thả (khoảng cách di chuyển tối thiểu 5px mới bắt đầu drag)
  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: {
        distance: 5,
      },
    })
  );

  // Gọi API lấy danh sách bàn ăn khi component mount
  useEffect(() => {
    fetchTables();
  }, []);

  const fetchTables = async () => {
    try {
      // Thực hiện call API từ .NET Web API
      // const response = await axiosClient.get('/tables');
      // setTables(response);
    } catch (error) {
      console.log('Đang dùng Mock Data bàn ăn do backend chưa sẵn sàng:', error);
    }
  };

  // Xử lý sự kiện kéo thả kết thúc (Drag End)
  const handleDragEnd = (event) => {
    const { active, delta } = event;
    if (!active || !delta) return;

    const tableId = active.data.current?.table?.id;
    if (!tableId) return;

    setTables((prevTables) =>
      prevTables.map((t) => {
        if (t.id === tableId) {
          // Tính toán tọa độ mới X, Y (giới hạn không âm)
          const newX = Math.max(0, (t.positionX || 0) + delta.x);
          const newY = Math.max(0, (t.positionY || 0) + delta.y);
          return { ...t, positionX: Math.round(newX), positionY: Math.round(newY) };
        }
        return t;
      })
    );
  };

  // Lưu sơ đồ vị trí bàn ăn lên Server
  const handleSaveLayout = async () => {
    setSavingLayout(true);
    try {
      // Gửi tọa độ vị trí X, Y mới tới API
      // await axiosClient.put('/tables/positions', tables.map(t => ({ id: t.id, positionX: t.positionX, positionY: t.positionY })));
      setTimeout(() => {
        setSavingLayout(false);
        setIsEditMode(false);
        alert('Đã lưu sơ đồ bàn ăn thành công!');
      }, 600);
    } catch (error) {
      console.error('Lỗi khi lưu sơ đồ:', error);
      setSavingLayout(false);
    }
  };

  // Cập nhật trạng thái bàn ăn
  const handleUpdateStatus = (newStatus) => {
    if (!selectedTable) return;
    setTables((prev) =>
      prev.map((t) => (t.id === selectedTable.id ? { ...t, status: newStatus } : t))
    );
    setSelectedTable((prev) => ({ ...prev, status: newStatus }));
  };

  // Thống kê số lượng bàn
  const stats = {
    total: tables.length,
    free: tables.filter((t) => t.status === 0).length,
    reserved: tables.filter((t) => t.status === 1).length,
    occupied: tables.filter((t) => t.status === 2).length,
    cleaning: tables.filter((t) => t.status === 3).length,
  };

  const filteredTables = selectedZone === 'All' 
    ? tables 
    : tables.filter((t) => t.zoneName === selectedZone);

  const zones = ['All', 'Khu A - Tầng 1', 'Phòng VIP', 'Sân thượng'];

  return (
    <div className="space-y-6">
      {/* Header Top & Action Bar */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 bg-slate-900/60 p-5 rounded-2xl border border-slate-800 backdrop-blur-md">
        <div>
          <h2 className="text-xl font-bold text-slate-100 flex items-center gap-2">
            <Layers className="w-6 h-6 text-orange-500" /> Sơ Đồ Bàn Ăn & Trạng Thái
          </h2>
          <p className="text-xs text-slate-400 mt-1">
            Quản lý vị trí kéo thả, mở bàn nhanh và theo dõi tình trạng ca trực
          </p>
        </div>

        {/* Action Controls */}
        <div className="flex items-center flex-wrap gap-2.5">
          {/* Switch View Mode */}
          <div className="flex bg-slate-800/80 p-1 rounded-xl border border-slate-700/60">
            <button
              onClick={() => setViewMode('canvas')}
              className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-semibold transition-all ${
                viewMode === 'canvas' ? 'bg-orange-500 text-white shadow-md' : 'text-slate-400 hover:text-white'
              }`}
            >
              <Move className="w-3.5 h-3.5" /> Sơ đồ Drag & Drop
            </button>
            <button
              onClick={() => setViewMode('grid')}
              className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-semibold transition-all ${
                viewMode === 'grid' ? 'bg-orange-500 text-white shadow-md' : 'text-slate-400 hover:text-white'
              }`}
            >
              <Grid className="w-3.5 h-3.5" /> Dạng Lưới
            </button>
          </div>

          {/* Toggle Edit Layout Drag & Drop */}
          {viewMode === 'canvas' && (
            isEditMode ? (
              <button
                onClick={handleSaveLayout}
                disabled={savingLayout}
                className="flex items-center gap-2 px-4 py-2 bg-emerald-600 hover:bg-emerald-500 text-white rounded-xl font-semibold text-xs transition-all shadow-lg shadow-emerald-600/20"
              >
                <Save className="w-4 h-4" />
                {savingLayout ? 'Đang lưu...' : 'Lưu vị trí sơ đồ'}
              </button>
            ) : (
              <button
                onClick={() => setIsEditMode(true)}
                className="flex items-center gap-2 px-4 py-2 bg-slate-800 hover:bg-slate-700 text-orange-400 border border-orange-500/30 rounded-xl font-semibold text-xs transition-all"
              >
                <Move className="w-4 h-4" /> Kéo thả vị trí
              </button>
            )
          )}
        </div>
      </div>

      {/* Stats Quick Badges */}
      <div className="grid grid-cols-2 sm:grid-cols-5 gap-3">
        <div className="bg-slate-900/60 border border-slate-800/80 rounded-xl p-3.5 flex items-center gap-3">
          <div className="w-10 h-10 rounded-lg bg-slate-800 flex items-center justify-center text-slate-300 font-bold">
            {stats.total}
          </div>
          <div>
            <p className="text-[11px] text-slate-400 font-medium">Tổng số bàn</p>
            <p className="text-sm font-bold text-slate-200">Tất cả khu vực</p>
          </div>
        </div>

        <div className="bg-emerald-950/20 border border-emerald-500/30 rounded-xl p-3.5 flex items-center gap-3">
          <div className="w-10 h-10 rounded-lg bg-emerald-500/20 text-emerald-400 flex items-center justify-center font-bold">
            {stats.free}
          </div>
          <div>
            <p className="text-[11px] text-emerald-400 font-semibold">0: Bàn Trống</p>
            <p className="text-xs text-slate-300">Sẵn sàng đón khách</p>
          </div>
        </div>

        <div className="bg-amber-950/20 border border-amber-500/30 rounded-xl p-3.5 flex items-center gap-3">
          <div className="w-10 h-10 rounded-lg bg-amber-500/20 text-amber-400 flex items-center justify-center font-bold">
            {stats.reserved}
          </div>
          <div>
            <p className="text-[11px] text-amber-400 font-semibold">1: Đã Đặt Trước</p>
            <p className="text-xs text-slate-300">Khách hẹn trước</p>
          </div>
        </div>

        <div className="bg-rose-950/20 border border-rose-500/30 rounded-xl p-3.5 flex items-center gap-3">
          <div className="w-10 h-10 rounded-lg bg-rose-500/20 text-rose-400 flex items-center justify-center font-bold">
            {stats.occupied}
          </div>
          <div>
            <p className="text-[11px] text-rose-400 font-semibold">2: Đang Có Khách</p>
            <p className="text-xs text-slate-300">Đang phục vụ</p>
          </div>
        </div>

        <div className="bg-slate-900/60 border border-slate-700/40 rounded-xl p-3.5 flex items-center gap-3 col-span-2 sm:col-span-1">
          <div className="w-10 h-10 rounded-lg bg-slate-800 text-slate-400 flex items-center justify-center font-bold">
            {stats.cleaning}
          </div>
          <div>
            <p className="text-[11px] text-slate-400 font-semibold">3: Đang Dọn Bàn</p>
            <p className="text-xs text-slate-300">Chờ làm sạch</p>
          </div>
        </div>
      </div>

      {/* Zone Selector Filter */}
      <div className="flex items-center gap-2 overflow-x-auto pb-1">
        <span className="text-xs text-slate-400 font-medium mr-2">Khu vực:</span>
        {zones.map((zone) => (
          <button
            key={zone}
            onClick={() => setSelectedZone(zone)}
            className={`px-3.5 py-1.5 rounded-xl text-xs font-semibold transition-all whitespace-nowrap ${
              selectedZone === zone
                ? 'bg-orange-500 text-white shadow-md shadow-orange-500/20'
                : 'bg-slate-900/60 text-slate-400 border border-slate-800 hover:text-slate-200'
            }`}
          >
            {zone === 'All' ? 'Tất cả các khu' : zone}
          </button>
        ))}
      </div>

      {/* Floor Canvas Area (Sơ đồ mặt bằng drag and drop) */}
      {viewMode === 'canvas' ? (
        <div className="relative min-h-[480px] bg-slate-900/40 border border-slate-800 rounded-3xl p-4 overflow-hidden bg-grid-pattern shadow-inner">
          {isEditMode && (
            <div className="absolute top-4 left-4 z-10 px-3 py-1.5 rounded-xl bg-orange-500/20 border border-orange-500/40 text-orange-300 text-xs font-semibold flex items-center gap-2 animate-pulse">
              <Move className="w-4 h-4" /> Chế độ Kéo thả tọa độ vị trí bàn ăn đang bật
            </div>
          )}

          <DndContext sensors={sensors} onDragEnd={handleDragEnd}>
            <div className="relative w-full h-[440px]">
              {filteredTables.map((table) => (
                <DraggableTableCard
                  key={table.id}
                  table={table}
                  isEditMode={isEditMode}
                  onClick={(t) => setSelectedTable(t)}
                />
              ))}
            </div>
          </DndContext>
        </div>
      ) : (
        /* Grid View Mode */
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
          {filteredTables.map((table) => {
            const statusInfo = TABLE_STATUS[table.status] || TABLE_STATUS[0];
            return (
              <div
                key={table.id}
                onClick={() => setSelectedTable(table)}
                className={`
                  p-5 rounded-2xl cursor-pointer transition-all duration-200 border shadow-lg
                  ${statusInfo.bgLight} ${statusInfo.border} hover:scale-102
                `}
              >
                <div className="flex items-center justify-between mb-3">
                  <h4 className="font-bold text-lg text-slate-100">{table.name}</h4>
                  <span className={`px-2.5 py-1 rounded-full text-xs font-bold text-white ${statusInfo.color}`}>
                    {statusInfo.label}
                  </span>
                </div>
                <div className="space-y-1.5 text-xs text-slate-300">
                  <p className="flex items-center gap-2">
                    <Users className="w-4 h-4 text-slate-400" /> Sức chứa: <strong className="text-white">{table.capacity} người</strong>
                  </p>
                  <p className="flex items-center gap-2">
                    <Layers className="w-4 h-4 text-slate-400" /> Vị trí: <strong className="text-slate-200">{table.zoneName}</strong>
                  </p>
                  {table.status === 2 && (
                    <p className="text-rose-400 font-bold text-sm pt-2">
                      Tạm tính: {table.totalAmount?.toLocaleString('vi-VN')} đ
                    </p>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      )}

      {/* Modal Chi tiết & Thao tác bàn ăn */}
      {selectedTable && (
        <div className="fixed inset-0 z-50 bg-slate-950/80 backdrop-blur-sm flex items-center justify-center p-4">
          <div className="bg-slate-900 border border-slate-800 rounded-3xl max-w-lg w-full p-6 space-y-6 shadow-2xl relative">
            {/* Modal Header */}
            <div className="flex items-center justify-between border-b border-slate-800 pb-4">
              <div>
                <span className="text-xs font-semibold text-orange-400 uppercase tracking-wider">
                  {selectedTable.zoneName}
                </span>
                <h3 className="text-xl font-bold text-white flex items-center gap-2">
                  {selectedTable.name}
                  <span className={`px-2.5 py-0.5 rounded-full text-xs font-bold text-white ${TABLE_STATUS[selectedTable.status]?.color}`}>
                    {TABLE_STATUS[selectedTable.status]?.label}
                  </span>
                </h3>
              </div>
              <button
                onClick={() => setSelectedTable(null)}
                className="p-2 rounded-xl text-slate-400 hover:text-white hover:bg-slate-800 transition-colors"
              >
                <X className="w-5 h-5" />
              </button>
            </div>

            {/* Change Status Fast Buttons */}
            <div>
              <label className="text-xs font-medium text-slate-400 mb-2 block">
                Cập nhật nhanh trạng thái bàn ăn:
              </label>
              <div className="grid grid-cols-2 sm:grid-cols-4 gap-2">
                {Object.entries(TABLE_STATUS).map(([key, info]) => {
                  const statusNum = Number(key);
                  const isCurrent = selectedTable.status === statusNum;
                  return (
                    <button
                      key={key}
                      onClick={() => handleUpdateStatus(statusNum)}
                      className={`
                        p-2.5 rounded-xl text-xs font-semibold border transition-all flex flex-col items-center gap-1
                        ${isCurrent 
                          ? `${info.color} text-white border-transparent shadow-lg` 
                          : 'bg-slate-800/60 border-slate-700 text-slate-300 hover:bg-slate-700'}
                      `}
                    >
                      <span>{info.label}</span>
                    </button>
                  );
                })}
              </div>
            </div>

            {/* Context Info Based on Status */}
            {selectedTable.status === 2 ? (
              <div className="bg-slate-800/50 rounded-2xl p-4 border border-slate-700/60 space-y-3">
                <div className="flex items-center justify-between text-xs text-slate-300">
                  <span>Giờ mở bàn: <strong>{selectedTable.checkInTime || '11:30'}</strong></span>
                  <span>Nhân viên ca: <strong>Trần Quốc Huy</strong></span>
                </div>
                <div className="border-t border-slate-700/50 pt-3">
                  <p className="text-xs font-semibold text-slate-400 mb-2">Món ăn đã gọi (Order):</p>
                  <ul className="space-y-1.5 text-xs text-slate-200">
                    <li className="flex justify-between">
                      <span>1x Bò Mỹ Nướng Đá Yakiniku</span>
                      <span className="font-semibold">320.000 đ</span>
                    </li>
                    <li className="flex justify-between">
                      <span>2x Bia Craft IPA 500ml</span>
                      <span className="font-semibold">130.000 đ</span>
                    </li>
                  </ul>
                  <div className="flex justify-between items-center mt-3 pt-2 border-t border-slate-700/50 text-sm">
                    <span className="font-bold text-slate-300">Tổng tạm tính:</span>
                    <span className="font-bold text-orange-400 text-base">
                      {(selectedTable.totalAmount || 450000).toLocaleString('vi-VN')} đ
                    </span>
                  </div>
                </div>
              </div>
            ) : selectedTable.status === 0 ? (
              <div className="p-4 bg-emerald-950/20 border border-emerald-500/30 rounded-2xl text-center space-y-2">
                <CheckCircle2 className="w-8 h-8 text-emerald-400 mx-auto" />
                <p className="text-sm font-semibold text-emerald-300">Bàn ăn hiện đang sẵn sàng</p>
                <p className="text-xs text-slate-400">Bạn có thể xếp khách vào bàn và tạo phiếu gọi món ngay.</p>
              </div>
            ) : null}

            {/* Modal Actions */}
            <div className="flex items-center justify-end gap-3 pt-2 border-t border-slate-800">
              <button
                onClick={() => setSelectedTable(null)}
                className="px-4 py-2.5 rounded-xl bg-slate-800 text-slate-300 hover:text-white font-medium text-xs transition-colors"
              >
                Đóng
              </button>
              {selectedTable.status === 0 && (
                <button
                  onClick={() => handleUpdateStatus(2)}
                  className="flex items-center gap-2 px-5 py-2.5 rounded-xl bg-gradient-to-r from-orange-600 to-orange-500 text-white font-bold text-xs shadow-lg shadow-orange-500/20 transition-all hover:scale-102"
                >
                  <Utensils className="w-4 h-4" /> Check-in Mở Bàn
                </button>
              )}
              {selectedTable.status === 2 && (
                <button
                  onClick={() => alert(`Chuyển đến trang thanh toán cho ${selectedTable.name}`)}
                  className="flex items-center gap-2 px-5 py-2.5 rounded-xl bg-gradient-to-r from-emerald-600 to-emerald-500 text-white font-bold text-xs shadow-lg shadow-emerald-500/20 transition-all hover:scale-102"
                >
                  <CreditCard className="w-4 h-4" /> Thanh Toán Hóa Đơn
                </button>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default TablesPage;
