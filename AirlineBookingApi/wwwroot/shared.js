/* ═══════════════════════════════════════════════
   SkyHmH — Shared JS (dùng chung cho mọi trang)
   ═══════════════════════════════════════════════ */
const API = '/api';

// ── Auth state (localStorage primary, sessionStorage read-only fallback) ──
function getToken() { return localStorage.getItem('token') || sessionStorage.getItem('token') || ''; }
function getRole() { return localStorage.getItem('role') || sessionStorage.getItem('role') || ''; }
function getUserName() { return localStorage.getItem('userName') || sessionStorage.getItem('userName') || ''; }
function isLoggedIn() { return !!getToken(); }
function isStaff() { return getRole() === 'Staff'; }

function saveAuth(data) {
  localStorage.setItem('token', data.token);
  localStorage.setItem('role', data.role);
  localStorage.setItem('userName', data.fullName || data.email);

  // cleanup any legacy session-scoped tokens left from older builds
  sessionStorage.removeItem('token');
  sessionStorage.removeItem('role');
  sessionStorage.removeItem('userName');
}
function clearAuth() {
  localStorage.removeItem('token');
  localStorage.removeItem('role');
  localStorage.removeItem('userName');

  sessionStorage.removeItem('token');
  sessionStorage.removeItem('role');
  sessionStorage.removeItem('userName');
}

// ── Pending auth callback (used by requireAuth) ──
let _pendingAuthCallback = null;
function setPendingAuthCallback(cb) { _pendingAuthCallback = cb; }

// ── API helper ──
function headers(json = true) {
  const h = {};
  if (json) h['Content-Type'] = 'application/json';
  const t = getToken();
  if (t) h['Authorization'] = `Bearer ${t}`;
  return h;
}
async function api(method, path, body) {
  const opts = { method, headers: headers(!!body) };
  if (body) opts.body = JSON.stringify(body);
  const res = await fetch(`${API}${path}`, opts);
  const text = await res.text();
  let data; try { data = JSON.parse(text); } catch { data = text; }
  if (!res.ok) {
    if (res.status === 401 && getToken() && !path.startsWith('/Auth/') && path !== '/Auth/me') {
      clearAuth();
      toast('Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.', 'warning');
      setTimeout(() => { window.location.href = '/'; }, 1500);
      throw new Error('SESSION_EXPIRED');
    }
    const msg = data?.errors ? Object.values(data.errors).flat().join('. ')
      : (data?.message || data?.title || `Lỗi ${res.status}`);
    throw new Error(msg);
  }
  return data;
}

async function apiSafe(method, path, body, msgOnError = '') {
  try {
    return await api(method, path, body);
  } catch (e) {
    toast(msgOnError || e.message, 'error');
    throw e;
  }
}

function setBtnLoading(btn, loading, loadingText = 'Đang xử lý...') {
  if (!btn) return;
  if (loading) {
    btn._oldText = btn._oldText || btn.innerHTML;
    btn.disabled = true;
    btn.classList.add('opacity-60');
    btn.innerHTML = loadingText;
  } else {
    btn.disabled = false;
    btn.classList.remove('opacity-60');
    if (btn._oldText) btn.innerHTML = btn._oldText;
  }
}

function askConfirm(message, title = 'Xác nhận') {
  return new Promise(resolve => {
    const old = document.getElementById('sky-confirm');
    if (old) old.remove();
    
    const div = document.createElement('div');
    div.id = 'sky-confirm';
    div.className = 'fixed inset-0 z-[300] flex items-center justify-center';
    div.innerHTML = `
      <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" id="sky-confirm-bg"></div>
      <div class="relative bg-white rounded-2xl shadow-2xl w-full max-w-sm mx-4 overflow-hidden transform transition-all duration-200 scale-95 opacity-0" id="sky-confirm-modal">
        <div class="p-6">
          <h3 class="text-lg font-bold text-zinc-900 mb-2" id="sky-confirm-title"></h3>
          <p class="text-sm text-zinc-600" id="sky-confirm-message"></p>
        </div>
        <div class="p-4 bg-zinc-50 border-t border-zinc-100 flex justify-end gap-2">
          <button id="sky-confirm-cancel" class="px-4 py-2 text-sm font-semibold text-zinc-600 hover:bg-zinc-200 rounded-lg transition-colors">Hủy</button>
          <button id="sky-confirm-ok" class="px-4 py-2 text-sm font-semibold bg-[#ff385c] hover:bg-[#e00b41] text-white rounded-lg transition-colors shadow-sm">Đồng ý</button>
        </div>
      </div>
    `;
    document.body.appendChild(div);
    
    document.getElementById('sky-confirm-title').textContent = title;
    document.getElementById('sky-confirm-message').textContent = message;
    
    requestAnimationFrame(() => {
      const modal = document.getElementById('sky-confirm-modal');
      modal.classList.remove('scale-95', 'opacity-0');
      modal.classList.add('scale-100', 'opacity-100');
    });

    const cleanup = (result) => {
      const modal = document.getElementById('sky-confirm-modal');
      modal.classList.remove('scale-100', 'opacity-100');
      modal.classList.add('scale-95', 'opacity-0');
      setTimeout(() => { div.remove(); resolve(result); }, 200);
    };

    document.getElementById('sky-confirm-cancel').onclick = () => cleanup(false);
    document.getElementById('sky-confirm-bg').onclick = () => cleanup(false);
    document.getElementById('sky-confirm-ok').onclick = () => cleanup(true);
  });
}

function askPrompt(message, title = 'Nhập thông tin') {
  return new Promise(resolve => {
    const old = document.getElementById('sky-prompt');
    if (old) old.remove();
    
    const div = document.createElement('div');
    div.id = 'sky-prompt';
    div.className = 'fixed inset-0 z-[300] flex items-center justify-center';
    div.innerHTML = `
      <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" id="sky-prompt-bg"></div>
      <div class="relative bg-white rounded-2xl shadow-2xl w-full max-w-sm mx-4 overflow-hidden transform transition-all duration-200 scale-95 opacity-0" id="sky-prompt-modal">
        <div class="p-6">
          <h3 class="text-lg font-bold text-zinc-900 mb-2" id="sky-prompt-title"></h3>
          <p class="text-sm text-zinc-600 mb-4" id="sky-prompt-message"></p>
          <input type="text" id="sky-prompt-input" class="w-full h-10 px-3 border border-zinc-300 rounded-lg focus:border-[#ff385c] focus:ring-1 focus:ring-[#ff385c] outline-none" />
        </div>
        <div class="p-4 bg-zinc-50 border-t border-zinc-100 flex justify-end gap-2">
          <button id="sky-prompt-cancel" class="px-4 py-2 text-sm font-semibold text-zinc-600 hover:bg-zinc-200 rounded-lg transition-colors">Hủy</button>
          <button id="sky-prompt-ok" class="px-4 py-2 text-sm font-semibold bg-[#ff385c] hover:bg-[#e00b41] text-white rounded-lg transition-colors shadow-sm">Xác nhận</button>
        </div>
      </div>
    `;
    document.body.appendChild(div);
    
    document.getElementById('sky-prompt-title').textContent = title;
    document.getElementById('sky-prompt-message').textContent = message;
    
    const input = document.getElementById('sky-prompt-input');
    
    requestAnimationFrame(() => {
      const modal = document.getElementById('sky-prompt-modal');
      modal.classList.remove('scale-95', 'opacity-0');
      modal.classList.add('scale-100', 'opacity-100');
      input.focus();
    });

    const cleanup = (result) => {
      const modal = document.getElementById('sky-prompt-modal');
      modal.classList.remove('scale-100', 'opacity-100');
      modal.classList.add('scale-95', 'opacity-0');
      setTimeout(() => { div.remove(); resolve(result); }, 200);
    };

    document.getElementById('sky-prompt-cancel').onclick = () => cleanup(null);
    document.getElementById('sky-prompt-bg').onclick = () => cleanup(null);
    document.getElementById('sky-prompt-ok').onclick = () => cleanup(input.value.trim());
    input.onkeydown = (e) => { if (e.key === 'Enter') cleanup(input.value.trim()); };
  });
}

// ── Format helpers ──
function fmt(n) { return new Intl.NumberFormat('vi-VN').format(n) + ' VND'; }
function fmtDate(s) {
  if (!s) return '';
  let date;
  if (s.includes('T')) {
    date = new Date(s);
  } else if (s.length === 10 && s.includes('-')) {
    const parts = s.split('-');
    date = new Date(parseInt(parts[0]), parseInt(parts[1]) - 1, parseInt(parts[2]));
  } else {
    date = new Date(s + (s.endsWith('Z') ? '' : 'Z'));
  }
  return date.toLocaleDateString('vi-VN', { timeZone: 'Asia/Ho_Chi_Minh' });
}
function fmtTime(s) { if (!s) return ''; return new Date(s + (s.endsWith('Z') ? '' : 'Z')).toLocaleTimeString('vi-VN', { timeZone: 'Asia/Ho_Chi_Minh', hour: '2-digit', minute: '2-digit' }); }
function fmtDateTime(s) { return fmtDate(s) + ' ' + fmtTime(s); }
function diffMin(a, b) {
  const toUtc = s => new Date(s + (s && !s.endsWith('Z') ? 'Z' : ''));
  return Math.round((toUtc(b) - toUtc(a)) / 60000);
}
function durStr(m) { const h = Math.floor(m / 60); return h > 0 ? `${h}h ${m % 60}m` : `${m}m`; }

// ── Toast notification ──
function ensureToast() {
  if (document.getElementById('sky-toast')) return;
  const t = document.createElement('div');
  t.id = 'sky-toast';
  t.className = 'fixed top-24 right-6 z-[200] px-6 py-3 rounded-xl shadow-lg font-medium text-sm transition-all duration-300 opacity-0 pointer-events-none translate-y-[-10px]';
  document.body.appendChild(t);
}
function toast(msg, type = 'info') {
  ensureToast();
  const t = document.getElementById('sky-toast');
  const colors = {
    success: 'bg-emerald-500 text-white',
    error: 'bg-red-500 text-white',
    info: 'bg-zinc-800 text-white',
    warning: 'bg-amber-500 text-white'
  };
  t.className = `fixed top-24 right-6 z-[200] px-6 py-3 rounded-xl shadow-lg font-medium text-sm transition-all duration-300 ${colors[type] || colors.info}`;
  t.textContent = msg;
  t.style.opacity = '1';
  t.style.transform = 'translateY(0)';
  clearTimeout(t._t);
  t._t = setTimeout(() => {
    t.style.opacity = '0';
    t.style.transform = 'translateY(-10px)';
  }, 3500);
}

// ── Auth Modals ──
function injectAuthModals() {
  if (document.getElementById('auth-modal-container')) return;
  const c = document.createElement('div');
  c.id = 'auth-modal-container';
  c.innerHTML = `
  <!-- Login Modal -->
  <div id="modal-login" class="hidden fixed inset-0 z-[100] flex items-center justify-center">
    <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" onclick="closeModal('modal-login')"></div>
    <div class="relative bg-white rounded-2xl shadow-2xl w-full max-w-md mx-4 overflow-hidden">
      <div class="p-6 border-b border-zinc-100 flex justify-between items-center">
        <h2 class="text-xl font-bold text-zinc-900">Đăng nhập</h2>
        <button onclick="closeModal('modal-login')" class="text-zinc-400 hover:text-zinc-800 p-1 rounded-full hover:bg-zinc-100 transition-colors">
          <span class="material-symbols-outlined">close</span>
        </button>
      </div>
      <form id="login-form" class="p-6 space-y-4" onsubmit="return false;">
        <div>
          <label class="block text-sm font-medium text-zinc-700 mb-1">Email</label>
          <input id="l-email" type="email" autocomplete="email" placeholder="you@example.com" class="w-full h-12 px-4 border border-zinc-300 rounded-lg focus:border-[#ff385c] focus:ring-1 focus:ring-[#ff385c] outline-none transition-colors"/>
        </div>
        <div>
          <label class="block text-sm font-medium text-zinc-700 mb-1">Mật khẩu</label>
          <input id="l-password" type="password" autocomplete="current-password" placeholder="••••••••" class="w-full h-12 px-4 border border-zinc-300 rounded-lg focus:border-[#ff385c] focus:ring-1 focus:ring-[#ff385c] outline-none transition-colors"/>
        </div>
        <div id="login-error" class="text-red-500 text-sm"></div>
        <button id="btn-login" type="submit" class="w-full h-12 bg-[#ff385c] hover:bg-[#e00b41] text-white font-semibold rounded-lg transition-colors">Đăng nhập</button>
        <p class="text-center text-sm text-zinc-500">Chưa có tài khoản? <a href="#" id="link-to-register" class="text-[#ff385c] font-medium hover:underline">Đăng ký</a></p>
      </form>
    </div>
  </div>
  <!-- Register Modal -->
  <div id="modal-register" class="hidden fixed inset-0 z-[100] flex items-center justify-center">
    <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" onclick="closeModal('modal-register')"></div>
    <div class="relative bg-white rounded-2xl shadow-2xl w-full max-w-md mx-4 overflow-hidden">
      <div class="p-6 border-b border-zinc-100 flex justify-between items-center">
        <h2 class="text-xl font-bold text-zinc-900">Đăng ký</h2>
        <button onclick="closeModal('modal-register')" class="text-zinc-400 hover:text-zinc-800 p-1 rounded-full hover:bg-zinc-100 transition-colors">
          <span class="material-symbols-outlined">close</span>
        </button>
      </div>
      <form id="register-form" class="p-6 space-y-4" onsubmit="return false;">
        <div>
          <label class="block text-sm font-medium text-zinc-700 mb-1">Họ tên</label>
          <input id="r-name" type="text" placeholder="Nguyễn Văn A" class="w-full h-12 px-4 border border-zinc-300 rounded-lg focus:border-[#ff385c] focus:ring-1 focus:ring-[#ff385c] outline-none transition-colors"/>
        </div>
        <div>
          <label class="block text-sm font-medium text-zinc-700 mb-1">Email</label>
          <input id="r-email" type="email" autocomplete="email" placeholder="you@example.com" class="w-full h-12 px-4 border border-zinc-300 rounded-lg focus:border-[#ff385c] focus:ring-1 focus:ring-[#ff385c] outline-none transition-colors"/>
        </div>
        <div>
          <label class="block text-sm font-medium text-zinc-700 mb-1">Số điện thoại</label>
          <input id="r-phone" type="tel" autocomplete="tel" placeholder="0901234567" class="w-full h-12 px-4 border border-zinc-300 rounded-lg focus:border-[#ff385c] focus:ring-1 focus:ring-[#ff385c] outline-none transition-colors"/>
        </div>
        <div>
          <label class="block text-sm font-medium text-zinc-700 mb-1">Mật khẩu</label>
          <input id="r-password" type="password" autocomplete="new-password" placeholder="Tối thiểu 8 ký tự, có chữ hoa, thường, số, ký hiệu" class="w-full h-12 px-4 border border-zinc-300 rounded-lg focus:border-[#ff385c] focus:ring-1 focus:ring-[#ff385c] outline-none transition-colors"/>
        </div>
        <div>
          <label class="block text-sm font-medium text-zinc-700 mb-1">Địa chỉ (tùy chọn)</label>
          <input id="r-address" type="text" placeholder="Đà Nẵng" class="w-full h-12 px-4 border border-zinc-300 rounded-lg focus:border-[#ff385c] focus:ring-1 focus:ring-[#ff385c] outline-none transition-colors"/>
        </div>
        <div id="register-error" class="text-red-500 text-sm"></div>
        <div id="register-success" class="text-emerald-600 text-sm"></div>
        <button id="btn-register" type="submit" class="w-full h-12 bg-[#ff385c] hover:bg-[#e00b41] text-white font-semibold rounded-lg transition-colors">Đăng ký</button>
        <p class="text-center text-sm text-zinc-500">Đã có tài khoản? <a href="#" id="link-to-login" class="text-[#ff385c] font-medium hover:underline">Đăng nhập</a></p>
      </form>
    </div>
  </div>`;
  document.body.appendChild(c);

  // Wire events
  document.getElementById('link-to-register').onclick = (e) => { e.preventDefault(); closeModal('modal-login'); openModal('modal-register'); };
  document.getElementById('link-to-login').onclick = (e) => { e.preventDefault(); closeModal('modal-register'); openModal('modal-login'); };

  document.getElementById('btn-login').onclick = async () => {
    const errEl = document.getElementById('login-error');
    errEl.textContent = '';
    try {
      const data = await api('POST', '/Auth/login', {
        email: document.getElementById('l-email').value,
        password: document.getElementById('l-password').value
      });
      saveAuth(data);
      closeModal('modal-login');
      toast('Đăng nhập thành công!', 'success');
      updateNavAuth();
      if (_pendingAuthCallback) {
        const cb = _pendingAuthCallback;
        _pendingAuthCallback = null;
        cb();
      } else {
        window.location.reload();
      }
    } catch (e) { errEl.textContent = e.message; }
  };

  document.getElementById('btn-register').onclick = async () => {
    const errEl = document.getElementById('register-error');
    const sucEl = document.getElementById('register-success');
    errEl.textContent = ''; sucEl.textContent = '';

    const password = document.getElementById('r-password').value;
    if (password.length < 8) { errEl.textContent = 'Mật khẩu phải có ít nhất 8 ký tự.'; return; }
    if (!/[A-Z]/.test(password)) { errEl.textContent = 'Mật khẩu phải chứa ít nhất 1 chữ hoa.'; return; }
    if (!/[a-z]/.test(password)) { errEl.textContent = 'Mật khẩu phải chứa ít nhất 1 chữ thường.'; return; }
    if (!/[0-9]/.test(password)) { errEl.textContent = 'Mật khẩu phải chứa ít nhất 1 chữ số.'; return; }
    if (!/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password)) { errEl.textContent = 'Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt.'; return; }

    try {
      await api('POST', '/Auth/register', {
        fullName: document.getElementById('r-name').value,
        email: document.getElementById('r-email').value,
        phoneNumber: document.getElementById('r-phone').value,
        password: document.getElementById('r-password').value,
        address: document.getElementById('r-address').value || null
      });
      sucEl.textContent = 'Đăng ký thành công! Hãy đăng nhập.';
    } catch (e) { errEl.textContent = e.message; }
  };
}

function openModal(id) { document.getElementById(id)?.classList.remove('hidden'); }
function closeModal(id) { document.getElementById(id)?.classList.add('hidden'); }

// ── Navbar rendering ──
function updateNavAuth() {
  const authBtns = document.getElementById('nav-auth-buttons');
  const userInfo = document.getElementById('nav-user-info');
  if (!authBtns || !userInfo) return;

  if (isLoggedIn()) {
    authBtns.classList.add('hidden');
    userInfo.classList.remove('hidden');
    const nameEl = document.getElementById('nav-user-name');
    if (nameEl) nameEl.textContent = getUserName();
    const roleEl = document.getElementById('nav-user-role');
    if (roleEl) roleEl.textContent = getRole();
  } else {
    authBtns.classList.remove('hidden');
    userInfo.classList.add('hidden');
  }
}

function renderNavbar(activePage = '') {
  const header = document.querySelector('header');
  if (!header) return;

  // Staff: các link quản lý tập trung. Customer: chuyến bay + check-in + lịch sử.
  let primaryLinks;
  if (isStaff()) {
    primaryLinks = `
      <a class="nav-item ${activePage === 'staff-booking' ? 'active' : ''}" href="/?staff=true">Đặt vé hộ</a>
      <a class="nav-item ${activePage === 'staff-checkin' ? 'active' : ''}" href="/staff.html?tab=checkin">Check-in</a>
      <a class="nav-item ${activePage === 'staff-cancel' ? 'active' : ''}" href="/staff.html?tab=cancel">Quản lý hủy</a>
      <a class="nav-item ${activePage === 'staff-flights' ? 'active' : ''}" href="/staff.html?tab=flights">Tình trạng chuyến bay</a>
      <a class="nav-item ${activePage === 'staff-report' ? 'active' : ''}" href="/staff.html?tab=report">Thống kê</a>
    `;
  } else {
    primaryLinks = `
      <a class="nav-item ${activePage === 'home' ? 'active' : ''}" href="/">Chuyến bay</a>
      <a class="nav-item ${activePage === 'checkin' ? 'active' : ''}" href="/checkin.html">Check-in trực tuyến</a>
      <a class="nav-item ${activePage === 'history' ? 'active' : ''}" href="/historytransaction.html">Lịch sử đặt vé</a>
    `;
  }

  header.innerHTML = `
  <div class="flex justify-between items-center h-20 px-6 md:px-20 w-full max-w-[1280px] mx-auto">
    <a href="/" class="text-[#ff385c] font-extrabold text-2xl tracking-tighter flex items-center gap-2 hover:opacity-80 transition-opacity">
      <span class="material-symbols-outlined" style="font-variation-settings:'FILL' 1">flight_takeoff</span>SkyHmH
    </a>
    <nav class="hidden md:flex items-center gap-6 h-full">
      ${primaryLinks}
    </nav>
    <div class="flex items-center gap-3">
      <div id="nav-auth-buttons" class="flex items-center gap-2 ${isLoggedIn() ? 'hidden' : ''}">
        <button onclick="openModal('modal-login')" class="text-zinc-700 hover:bg-zinc-100 px-4 py-2 rounded-full text-sm font-semibold transition-colors">Đăng nhập</button>
        <button onclick="openModal('modal-register')" class="bg-[#ff385c] hover:bg-[#e00b41] text-white px-4 py-2 rounded-full text-sm font-semibold transition-colors shadow-sm">Đăng ký</button>
      </div>
      <div id="nav-user-info" class="flex items-center gap-3 ${isLoggedIn() ? '' : 'hidden'}">
        <a href="/profile.html" class="flex items-center gap-2 bg-zinc-50 border border-zinc-200 rounded-full px-4 py-2 hover:bg-zinc-100 transition-colors ${activePage === 'profile' ? 'ring-2 ring-[#ff385c]' : ''}" title="Hồ sơ cá nhân">
          <span class="material-symbols-outlined text-[20px] text-[#ff385c]">person</span>
          <span id="nav-user-name" class="text-sm font-semibold text-zinc-800 hover:underline">${esc(getUserName())}</span>
          <span id="nav-user-role" class="text-xs text-zinc-500 bg-zinc-200 px-2 py-0.5 rounded-full">${esc(getRole())}</span>
        </a>
        <button onclick="logout()" class="text-zinc-500 hover:text-red-500 hover:bg-zinc-100 p-2 rounded-full transition-colors" title="Đăng xuất">
          <span class="material-symbols-outlined text-[20px]">logout</span>
        </button>
      </div>
    </div>
  </div>`;

  // Add nav-item styles
  if (!document.getElementById('nav-item-styles')) {
    const s = document.createElement('style');
    s.id = 'nav-item-styles';
    s.textContent = `
      .nav-item { display:flex;align-items:center;height:100%;padding:0 12px;font-size:14px;font-weight:600;color:#71717a;transition:all .2s;border-bottom:2px solid transparent; }
      .nav-item:hover { color:#18181b;background:rgba(0,0,0,.02); }
      .nav-item.active { color:#18181b;border-bottom-color:#ff385c; }
    `;
    document.head.appendChild(s);
  }
}

function logout() {
  clearAuth();
  toast('Đã đăng xuất', 'info');
  window.location.href = '/';
}

function requireAuth(msg = 'Vui lòng đăng nhập để tiếp tục', onAuthed = null) {
  if (!isLoggedIn()) {
    toast(msg, 'warning');
    if (onAuthed) setPendingAuthCallback(onAuthed);
    openModal('modal-login');
    return false;
  }
  return true;
}

// ── URL params helper ──
function getUrlParam(name) {
  return new URLSearchParams(window.location.search).get(name);
}

// ── Check-in window helper ──
function isInCheckInWindow(departureTimeStr) {
  if (!departureTimeStr) return false;
  const nowUtc = new Date();
  const depUtc = new Date(departureTimeStr + (departureTimeStr.endsWith('Z') ? '' : 'Z'));
  const hoursUntilDep = (depUtc - nowUtc) / 3600000;
  return hoursUntilDep >= 1 && hoursUntilDep <= 24;
}

// ── Status labels (Vietnamese) ──
const STATUS_LABELS = {
  PendingPayment: 'Chờ thanh toán',
  Paid: 'Đã thanh toán',
  CheckedIn: 'Đã check-in',
  Cancelled: 'Đã hủy',
  CancelRequested: 'Đang chờ hủy',
  Confirmed: 'Đã xác nhận',
  Failed: 'Thất bại'
};
function statusLabel(s) { return STATUS_LABELS[s] || s; }

// ── Airport data helper ──
async function loadAirports() {
  try {
    const airports = await api('GET', '/Flights/airports');
    if (Array.isArray(airports) && airports.length > 0) {
      return airports.map(a => ({ code: a, name: a }));
    }
  } catch (e) {
    // fallback to static list
  }
  return [
    { code: 'Da Nang', name: 'Đà Nẵng (DAD)' },
    { code: 'Ho Chi Minh', name: 'TP. Hồ Chí Minh (SGN)' },
    { code: 'Ha Noi', name: 'Hà Nội (HAN)' },
    { code: 'Bangkok', name: 'Bangkok (BKK)' },
    { code: 'Singapore', name: 'Singapore (SIN)' }
  ];
}

// ── Airport label map (shared across pages) ──
const AIRPORT_LABEL_MAP = {
  // Old keys (không dấu) — giữ để backward compat
  'Da Nang': 'Đà Nẵng (DAD)',
  'Ho Chi Minh': 'TP. Hồ Chí Minh (SGN)',
  'Ha Noi': 'Hà Nội (HAN)',
  'Can Tho': 'Cần Thơ (VCA)',
  'Hue': 'Huế (HUI)',
  'Phu Quoc': 'Phú Quốc (PQC)',
  'Nha Trang': 'Nha Trang (CXR)',
  'Hai Phong': 'Hải Phòng (HPH)',
  'Bangkok': 'Bangkok (BKK)',
  'Singapore': 'Singapore (SIN)',
  'Tokyo': 'Tokyo (NRT)',
  'Seoul': 'Seoul (ICN)',
  'Kuala Lumpur': 'Kuala Lumpur (KUL)',
  // New keys (có dấu) — khớp với giá trị DB hiện tại
  'Đà Nẵng': 'Đà Nẵng (DAD)',
  'TP. Hồ Chí Minh': 'TP. Hồ Chí Minh (SGN)',
  'Hà Nội': 'Hà Nội (HAN)',
  'Cần Thơ': 'Cần Thơ (VCA)',
  'Huế': 'Huế (HUI)',
  'Phú Quốc': 'Phú Quốc (PQC)',
  'Nha Trang': 'Nha Trang (CXR)',
  'Hải Phòng': 'Hải Phòng (HPH)',
  // Legacy full-label keys
  'Da Nang (DAD)': 'Đà Nẵng (DAD)',
  'TP. Hồ Chí Minh (SGN)': 'TP. Hồ Chí Minh (SGN)',
  'Hà Nội (HAN)': 'Hà Nội (HAN)'
};

function getAirportLabel(code) {
  return AIRPORT_LABEL_MAP[code] || code;
}

function normalizeAirportName(s) {
  if (!s) return '';
  return s.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase();
}

// ── XSS escape ──
function esc(s) {
  return String(s)
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;')
    .replaceAll("'", '&#39;');
}

const escapeHtmlText = esc;

// ── Vietnam time helpers ──
function getVietnamHour(dateStr) {
  if (!dateStr) return 0;
  return parseInt(new Date(dateStr).toLocaleString('en-US', { timeZone: 'Asia/Ho_Chi_Minh', hour: 'numeric', hour12: false }));
}

function getVietnamDate(dateStr) {
  if (!dateStr) return '';
  return new Date(dateStr).toLocaleDateString('vi-VN', { timeZone: 'Asia/Ho_Chi_Minh' });
}

// ── Airline logo helper ──
function airlineLogoHtml(code, name, color) {
  const safeCode = esc(code || 'HMH');
  const safeColor = esc(color || '#6a6a6a');
  const safeName = esc(name || safeCode);
  const imgSrc = `/images/airline/${safeCode}.png`;
  const initial = safeCode.charAt(0);
  const fallbackHtml = `<div class='w-10 h-10 rounded-full flex items-center justify-center text-white font-bold text-sm' style='background-color:${safeColor}'>${initial}</div>`;
  const encodedFallback = encodeURIComponent(fallbackHtml);

  return `<img src="${imgSrc}" alt="${safeName}" class="w-10 h-10 rounded-full object-cover border border-zinc-200 bg-white" onerror="this.onerror=null;this.outerHTML=decodeURIComponent('${encodedFallback}')" />`;
}

// ── Init on every page ──
document.addEventListener('DOMContentLoaded', () => {
  injectAuthModals();
});
