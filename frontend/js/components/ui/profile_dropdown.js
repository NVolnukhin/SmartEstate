const profileDropdown = document.createElement('div');
profileDropdown.className = 'profile-dropdown';
profileDropdown.innerHTML = `
    <a href="./compares_history.html">История сравнений</a>
    <a href="./favorite.html">Избранное</a>
    <a href="./profile.html">Настройки</a>
    <a href="#" class="logout-btn">Выйти</a>
`;
document.body.appendChild(profileDropdown);

document.querySelector('.profile-link').addEventListener('click', function(e) {
    e.preventDefault();
    profileDropdown.classList.toggle('show');
});

document.addEventListener('click', function(e) {
    if (!e.target.closest('.profile-link-wrapper') && 
        !e.target.closest('.profile-dropdown')) {
        profileDropdown.classList.remove('show');
    }
});

document.querySelector('.logout-btn').addEventListener('click', function(e) {
    e.preventDefault();
    localStorage.removeItem('authToken');
    window.location.href = './index.html';
});