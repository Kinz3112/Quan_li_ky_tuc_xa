document.addEventListener('DOMContentLoaded', () => {
    const messageDiv = document.getElementById('message');

    function getUsers() {
        const users = localStorage.getItem('users');
        return users ? JSON.parse(users) : [];
    }

    function saveUsers(users) {
        localStorage.setItem('users', JSON.stringify(users));
    }

    function generateId() {
        return Date.now() + Math.random().toString(36).substr(2, 9);
    }

    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', (e) => {
            e.preventDefault();
            const username = e.target.username.value.trim();
            const email = e.target.email.value.trim();
            const password = e.target.password.value;
            const confirmPassword = e.target.confirmPassword.value;

            if (!username || !email || !password) {
                showMessage('Vui lòng điền đầy đủ thông tin!', 'error');
                return;
            }

            if (password !== confirmPassword) {
                showMessage('Mật khẩu không khớp!', 'error');
                return;
            }

            if (password.length < 6) {
                showMessage('Mật khẩu phải có ít nhất 6 ký tự!', 'error');
                return;
            }

            const users = getUsers();

            const existingUser = users.find(user =>
                user.email === email || user.username === username
            );

            if (existingUser) {
                showMessage('Email hoặc tên đăng nhập đã tồn tại!', 'error');
                return;
            }


            const newUser = {
                id: generateId(),
                username,
                email,
                password,
                createdAt: new Date().toISOString()
            };

            users.push(newUser);
            saveUsers(users);

            showMessage('Đăng ký thành công!', 'success');
            setTimeout(() => window.location.href = 'login.html', 1500);
        });
    }

    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', (e) => {
            e.preventDefault();
            const email = e.target.email.value.trim();
            const password = e.target.password.value;

            if (!email || !password) {
                showMessage('Vui lòng điền đầy đủ thông tin!', 'error');
                return;
            }

            const users = getUsers();
            const user = users.find(u =>
                (u.email === email || u.username === email) && u.password === password
            );

            if (!user) {
                showMessage('Email/tên đăng nhập hoặc mật khẩu không đúng!', 'error');
                return;
            }

            localStorage.setItem('currentUser', JSON.stringify({
                id: user.id,
                username: user.username,
                email: user.email,
                loginTime: new Date().toISOString()
            }));

            showMessage('Đăng nhập thành công!', 'success');
            setTimeout(() => window.location.href = 'index.html', 1000);
        });
    }

    function showMessage(message, type) {
        if (messageDiv) {
            messageDiv.textContent = message;
            messageDiv.className = `message ${type}`;
        }
    }

    const userInfo = document.getElementById('userInfo');
    const dashboardActions = document.getElementById('dashboardActions');

    if (userInfo) {
        const currentUser = localStorage.getItem('currentUser');
        if (!currentUser) {
            userInfo.innerHTML = `
                <p>Bạn chưa đăng nhập vào hệ thống</p>
            `;
            if (dashboardActions) {
                dashboardActions.innerHTML = `
                    <button onclick="window.location.href='login.html'">Đăng Nhập</button>
                    <button onclick="window.location.href='register.html'">Đăng Ký</button>
                `;
            }
        } else {
            const user = JSON.parse(currentUser);
            userInfo.innerHTML = `
                <p><strong>ID:</strong> ${user.id}</p>
                <p><strong>Tên đăng nhập:</strong> ${user.username}</p>
                <p><strong>Email:</strong> ${user.email}</p>
                <p><strong>Đăng nhập lúc:</strong> ${new Date(user.loginTime).toLocaleString('vi-VN')}</p>
            `;

            if (dashboardActions) {
                dashboardActions.innerHTML = `
                    <button id="logoutButton">Đăng Xuất</button>
                `;
                const logoutBtn = document.getElementById('logoutButton');
                if (logoutBtn) {
                    logoutBtn.addEventListener('click', () => {
                        localStorage.removeItem('currentUser');
                        showMessage('Đã đăng xuất thành công!', 'success');
                        setTimeout(() => window.location.reload(), 1000);
                    });
                }
            }
        }
    }


});
