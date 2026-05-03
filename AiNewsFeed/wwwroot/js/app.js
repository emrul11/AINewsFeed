// ==================== STATE ====================
let currentPage = 1;
let currentSource = '';
let currentModel = '';
let currentCompany = '';
let currentUnreadOnly = false;
let currentSearch = '';
let isLoading = false;
let hasMore = true;
let lastRefreshTime = null;
let searchTimeout = null;
let isRefreshing = false;
const pageSize = 50;

const sourceColors = {};
const colorPalette = [
    '#0d6efd', '#6610f2', '#6f42c1', '#d63384', '#dc3545',
    '#fd7e14', '#ffc107', '#198754', '#20c997', '#0dcaf0',
    '#e83e8c', '#6c757d', '#7952b3', '#17a2b8', '#28a745'
];

// ==================== DOM ====================
const articlesContainer = document.getElementById('articlesContainer');
const refreshBtn = document.getElementById('refreshBtn');
const refreshSpinner = document.getElementById('refreshSpinner');
const refreshIcon = document.getElementById('refreshIcon');
const markAllReadBtn = document.getElementById('markAllReadBtn');
const statsBadge = document.getElementById('statsBadge');
const sourceFilter = document.getElementById('sourceFilter');
const modelFilter = document.getElementById('modelFilter');
const companyFilter = document.getElementById('companyFilter');
const unreadOnlyToggle = document.getElementById('unreadOnlyToggle');
const searchInput = document.getElementById('searchInput');
const clearSearchBtn = document.getElementById('clearSearchBtn');
const loadMoreBtn = document.getElementById('loadMoreBtn');
const lastRefreshed = document.getElementById('lastRefreshed');
const footerCount = document.getElementById('footerCount');
const toastEl = document.getElementById('mainToast');
const toastBody = document.getElementById('toastBody');
const toast = new bootstrap.Toast(toastEl);

// ==================== HELPERS ====================
function showToast(message, type = 'info') {
    toastBody.textContent = message;
    toastEl.className = 'toast align-items-center text-white';
    if (type === 'success') toastEl.classList.add('bg-success');
    else if (type === 'error') toastEl.classList.add('bg-danger');
    else if (type === 'warning') toastEl.classList.add('bg-warning', 'text-dark');
    else toastEl.classList.add('bg-secondary');
    toast.show();
}

function getSourceColor(sourceName) {
    if (!sourceColors[sourceName]) {
        const index = Object.keys(sourceColors).length % colorPalette.length;
        sourceColors[sourceName] = colorPalette[index];
    }
    return sourceColors[sourceName];
}

function formatDate(dateString) {
    if (!dateString) return 'Unknown';
    const date = new Date(dateString);
    if (isNaN(date.getTime())) return 'Invalid date';
    const now = new Date();
    const diffMs = now - date;
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return 'Just now';
    if (diffMins < 60) return `${diffMins}m ago`;
    if (diffHours < 24) return `${diffHours}h ago`;
    if (diffDays === 1) return 'Yesterday';
    if (diffDays < 7) return `${diffDays}d ago`;

    const options = { month: 'short', day: 'numeric' };
    if (date.getFullYear() !== now.getFullYear()) options.year = 'numeric';
    return date.toLocaleDateString('en-US', options);
}

function truncate(str, len) {
    if (!str) return '';
    return str.length > len ? str.substring(0, len) + '...' : str;
}

function parseJsonArray(jsonStr) {
    if (!jsonStr) return [];
    try {
        const parsed = JSON.parse(jsonStr);
        return Array.isArray(parsed) ? parsed : [];
    } catch {
        return [];
    }
}

// ==================== API ====================
async function fetchArticles(reset = false) {
    if (isLoading) return;
    isLoading = true;

    if (reset) {
        currentPage = 1;
        hasMore = true;
        articlesContainer.innerHTML = '';
    }

    const params = new URLSearchParams({
        page: currentPage,
        pageSize: pageSize
    });
    if (currentSource) params.append('source', currentSource);
    if (currentModel) params.append('model', currentModel);
    if (currentCompany) params.append('company', currentCompany);
    if (currentUnreadOnly) params.append('unread', 'true');
    if (currentSearch) params.append('search', currentSearch);

    try {
        const response = await fetch(`/api/articles?${params}`);
        if (!response.ok) throw new Error(`HTTP ${response.status}`);

        const responseData = await response.json();

        // Handle BOTH paged object {items, totalCount} AND old array format
        const articles = responseData.items || responseData || [];
        const totalCount = responseData.totalCount
            || parseInt(response.headers.get('X-Total-Count'))
            || articles.length;

        if (!articles.length) {
            hasMore = false;
            if (reset) {
                articlesContainer.innerHTML = `
                    <div class="text-center text-muted py-5">
                        <i class="bi bi-inbox fs-1"></i>
                        <p class="mt-2">No articles found.</p>
                    </div>`;
            }
        } else {
            renderArticles(articles);
            hasMore = articles.length === pageSize;
        }

        loadMoreBtn.classList.toggle('visible', hasMore && articles.length > 0);

        const visibleUnread = articles.filter(a => !a.isRead).length;
        updateStats(totalCount, visibleUnread);
        footerCount.textContent = `${totalCount} total`;

    } catch (err) {
        console.error('Fetch articles error:', err);
        if (reset) {
            articlesContainer.innerHTML = `
                <div class="text-center text-danger py-5">
                    <i class="bi bi-exclamation-triangle fs-1"></i>
                    <p class="mt-2">Failed to load articles. Is the API running?</p>
                    <small class="text-muted">${err.message}</small>
                </div>`;
        }
        showToast('Failed to load articles: ' + err.message, 'error');
    } finally {
        isLoading = false;
    }
}

async function fetchSources() {
    try {
        const response = await fetch('/api/sources');
        if (!response.ok) throw new Error();
        const sources = await response.json();

        sourceFilter.innerHTML = '<option value="">All Sources</option>';
        sources.filter(s => s.isActive).forEach(s => {
            const opt = document.createElement('option');
            opt.value = s.name;
            opt.textContent = s.name;
            sourceFilter.appendChild(opt);
        });
    } catch (err) {
        console.error('Fetch sources error:', err);
    }
}

async function fetchModels() {
    try {
        const response = await fetch('/api/articles/models');
        if (!response.ok) return;
        const models = await response.json();

        modelFilter.innerHTML = '<option value="">All Models</option>';
        Object.entries(models)
            .sort((a, b) => b[1] - a[1])
            .forEach(([name, count]) => {
                const opt = document.createElement('option');
                opt.value = name;
                opt.textContent = `${name.replace(/-/g, ' ').toUpperCase()} (${count})`;
                modelFilter.appendChild(opt);
            });
    } catch (err) {
        console.error('Fetch models error:', err);
    }
}

async function fetchCompanies() {
    try {
        const response = await fetch('/api/articles/companies');
        if (!response.ok) return;
        const companies = await response.json();

        companyFilter.innerHTML = '<option value="">All Companies</option>';
        Object.entries(companies)
            .sort((a, b) => b[1] - a[1])
            .forEach(([name, count]) => {
                const opt = document.createElement('option');
                opt.value = name;
                opt.textContent = `${name.toUpperCase()} (${count})`;
                companyFilter.appendChild(opt);
            });
    } catch (err) {
        console.error('Fetch companies error:', err);
    }
}

async function refreshFeeds() {
    if (isRefreshing) return;
    setRefreshLoading(true);

    try {
        const response = await fetch('/api/feeds/refresh', { method: 'POST' });
        if (!response.ok) throw new Error(`HTTP ${response.status}`);

        const result = await response.json();
        lastRefreshTime = new Date();
        updateLastRefreshed();

        let msg = `Added ${result.newArticlesAdded || 0} new articles.`;
        if (result.failedSources > 0) msg += ` ${result.failedSources} source(s) failed.`;
        if (result.disabledSources > 0) msg += ` ${result.disabledSources} source(s) disabled.`;

        showToast(msg, result.failedSources > 0 ? 'warning' : 'success');

        await fetchArticles(true);
        await fetchSources();
        await fetchModels();
        await fetchCompanies();

    } catch (err) {
        console.error('Refresh error:', err);
        showToast('Refresh failed: ' + err.message, 'error');
    } finally {
        setRefreshLoading(false);
    }
}

async function markArticleRead(id, isRead) {
    try {
        const response = await fetch(`/api/articles/${id}/read`, {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ isRead })
        });
        return response.ok;
    } catch (err) {
        console.error('Mark read error:', err);
        return false;
    }
}

async function markAllRead() {
    try {
        const response = await fetch('/api/articles/read-all', { method: 'PATCH' });
        if (!response.ok) throw new Error();
        const result = await response.json();
        showToast(`Marked ${result.markedCount} articles as read.`, 'success');
        await fetchArticles(true);
    } catch (err) {
        showToast('Failed to mark all as read.', 'error');
    }
}

// ==================== RENDERING ====================
function renderArticles(articles) {
    articles.forEach(article => {
        const col = document.createElement('div');
        col.className = 'col-12';

        const color = getSourceColor(article.source);
        const readClass = article.isRead ? 'read' : 'unread';
        const hasContent = article.content && article.content.length > (article.summary?.length || 0);

        const models = parseJsonArray(article.mentionedModels);
        const companies = parseJsonArray(article.mentionedCompanies);

        const modelTags = models.map(m =>
            `<span class="model-badge me-1">${m.replace(/-/g, ' ').toUpperCase()}</span>`
        ).join('');

        const companyTags = companies.map(c =>
            `<span class="company-badge me-1">${c.toUpperCase()}</span>`
        ).join('');

        col.innerHTML = `
            <div class="article-card p-3 p-md-4" data-id="${article.id}">
                <div class="d-flex align-items-start gap-3">
                    <div class="form-check mt-1">
                        <input class="form-check-input" type="checkbox"
                            ${article.isRead ? 'checked' : ''}
                            title="${article.isRead ? 'Mark unread' : 'Mark read'}"
                            onchange="handleCheckboxChange(${article.id}, this.checked)">
                    </div>
                    <div class="flex-grow-1 min-width-0">
                        <div class="d-flex align-items-center gap-2 mb-2 flex-wrap">
                            <span class="source-badge text-white" style="background-color: ${color};">${article.source}</span>
                            <span class="date-text"><i class="bi bi-clock me-1"></i>${formatDate(article.publishedAt)}</span>
                            ${article.author ? `<span class="date-text"><i class="bi bi-person me-1"></i>${truncate(article.author, 20)}</span>` : ''}
                        </div>
                        <div class="mb-2">${modelTags}${companyTags}</div>
                        <a href="${article.url}" target="_blank" rel="noopener"
                           class="article-title ${readClass} d-block mb-2"
                           onclick="handleTitleClick(${article.id}, event)">
                            ${article.title}
                        </a>
                        ${article.summary ? `<p class="summary-text mb-0">${truncate(article.summary, 220)}</p>` : ''}
                        ${hasContent ? `
                            <button class="btn btn-link btn-sm text-decoration-none ps-0 mt-2 text-muted"
                                    onclick="toggleContent(this, ${article.id})">
                                <i class="bi bi-chevron-down"></i> Read more
                            </button>
                            <div class="content-expand mt-2" id="content-${article.id}">
                                <div class="p-3 rounded" style="background-color: #12141a; border: 1px solid var(--border-color);">
                                    ${article.content}
                                </div>
                            </div>
                        ` : ''}
                    </div>
                </div>
            </div>
        `;
        articlesContainer.appendChild(col);
    });
}

function updateStats(total, unread) {
    statsBadge.textContent = `${total} articles | ${unread} unread`;
}

function updateLastRefreshed() {
    lastRefreshed.textContent = lastRefreshTime ? formatDate(lastRefreshTime) : 'never';
}

function setRefreshLoading(loading) {
    isRefreshing = loading;
    refreshBtn.disabled = loading;
    refreshSpinner.classList.toggle('d-none', !loading);
    refreshIcon.classList.toggle('d-none', loading);
}

// ==================== EVENT HANDLERS ====================
window.handleCheckboxChange = async function (id, checked) {
    const success = await markArticleRead(id, checked);
    if (success) {
        const titleEl = document.querySelector(`[data-id="${id}"] .article-title`);
        if (titleEl) {
            titleEl.classList.remove('unread', 'read');
            titleEl.classList.add(checked ? 'read' : 'unread');
        }
    }
};

window.handleTitleClick = async function (id, event) {
    const success = await markArticleRead(id, true);
    if (success) {
        const titleEl = event.target;
        titleEl.classList.remove('unread');
        titleEl.classList.add('read');
        const checkbox = document.querySelector(`[data-id="${id}"] input[type="checkbox"]`);
        if (checkbox) checkbox.checked = true;
    }
};

window.toggleContent = function (btn, id) {
    const content = document.getElementById(`content-${id}`);
    const icon = btn.querySelector('i');
    const isExpanded = content.classList.contains('show');

    if (isExpanded) {
        content.classList.remove('show');
        icon.className = 'bi bi-chevron-down';
        btn.innerHTML = btn.innerHTML.replace('Read less', 'Read more');
    } else {
        content.classList.add('show');
        icon.className = 'bi bi-chevron-up';
        btn.innerHTML = btn.innerHTML.replace('Read more', 'Read less');
    }
};

refreshBtn.addEventListener('click', refreshFeeds);
markAllReadBtn.addEventListener('click', markAllRead);

sourceFilter.addEventListener('change', (e) => {
    currentSource = e.target.value;
    fetchArticles(true);
});

modelFilter.addEventListener('change', (e) => {
    currentModel = e.target.value;
    fetchArticles(true);
});

companyFilter.addEventListener('change', (e) => {
    currentCompany = e.target.value;
    fetchArticles(true);
});

unreadOnlyToggle.addEventListener('change', (e) => {
    currentUnreadOnly = e.target.checked;
    fetchArticles(true);
});

searchInput.addEventListener('input', (e) => {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(() => {
        currentSearch = e.target.value;
        fetchArticles(true);
    }, 400);
});

clearSearchBtn.addEventListener('click', () => {
    searchInput.value = '';
    currentSearch = '';
    fetchArticles(true);
});

loadMoreBtn.addEventListener('click', () => {
    if (!isLoading && hasMore) {
        currentPage++;
        fetchArticles(false);
    }
});

// Keyboard shortcuts
document.addEventListener('keydown', (e) => {
    if (e.target.tagName === 'INPUT' || e.target.tagName === 'TEXTAREA') return;
    if (e.key.toLowerCase() === 'r') {
        e.preventDefault();
        refreshFeeds();
    } else if (e.key.toLowerCase() === 'm') {
        e.preventDefault();
        markAllRead();
    }
});

// ==================== INIT ====================
async function init() {
    await fetchSources();
    await fetchModels();
    await fetchCompanies();
    await fetchArticles(true);
    updateLastRefreshed();
    setInterval(updateLastRefreshed, 60000);
}

init();