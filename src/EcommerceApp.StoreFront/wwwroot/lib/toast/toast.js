const notificationTypes = [
    { type: 'success', icon: 'check-circle', title: 'Success!' },
    { type: 'warning', icon: 'alert-triangle', title: 'Warning!' },
    { type: 'info', icon: 'info', title: 'Information' },
    { type: 'error', icon: 'alert-circle', title: 'Error!' }
];

// Keep track of notifications for proper ordering
let notifications = [];
let notificationCount = 0;
const MAX_VISIBLE_NOTIFICATIONS = 3; // Maximum number of notifications to show when hovering

function createNotification(type, customMessage = null, customTitle = null) {
    const { icon, title } = notificationTypes.find(n => n.type === type);
    const id = `notification-${Date.now()}-${notificationCount++}`;

    const notificationElement = document.createElement('div');
    notificationElement.id = id;
    notificationElement.className = `notification notification-${type} text-white p-4 rounded-lg shadow-lg flex items-center justify-between max-w-sm w-full animate-slide-in`;
    notificationElement.setAttribute('role', 'alert');
    notificationElement.setAttribute('aria-live', 'assertive');

    notificationElement.innerHTML = `
        <div class="flex items-center space-x-3">
            <i data-feather="${icon}" class="w-6 h-6" aria-hidden="true"></i>
            <div>
                <h3 class="font-bold p-0 text-white text-sm">${customTitle || title}</h3>
                <p class="text-sm p-0">${customMessage || ' '}</p>
            </div>
        </div>
        <button class="text-white hover:text-gray-200 transition-colors" aria-label="Close notification">
            <i data-feather="x" class="w-5 h-5"></i>
        </button>
    `;

    const closeButton = notificationElement.querySelector('button');
    closeButton.addEventListener('click', (e) => {
        e.stopPropagation();
        removeNotification(id);
    });

    // Add to our tracking array
    notifications.push({
        id,
        element: notificationElement,
        timestamp: Date.now()
    });

    // Sort notifications by timestamp (oldest first)
    notifications.sort((a, b) => a.timestamp - b.timestamp);

    updateNotificationsDisplay();

    if (typeof feather !== 'undefined') {
        feather.replace({ scope: notificationElement });
    }

    // Auto-remove after 5 seconds
    setTimeout(() => {
        removeNotification(id);
    }, 5000);
}

function updateNotificationsDisplay() {
    // Clear container
    const container = document.getElementById('notifications-container');
    container.innerHTML = '';

    // Add stacked indicator to the first notification if there are more than one
    if (notifications.length > 1) {
        notifications[0].element.classList.add('stacked-indicator');
    } else {
        notifications[0]?.element.classList.remove('stacked-indicator');
    }

    // Add all notifications to the container
    notifications.forEach((notification, index) => {
        // Add hidden-notification class to notifications beyond the MAX_VISIBLE_NOTIFICATIONS
        if (index >= MAX_VISIBLE_NOTIFICATIONS) {
            notification.element.classList.add('hidden-notification');
        } else {
            notification.element.classList.remove('hidden-notification');
        }

        container.appendChild(notification.element);
    });

    // Add counter badge if there are more notifications than MAX_VISIBLE_NOTIFICATIONS
    if (notifications.length > MAX_VISIBLE_NOTIFICATIONS) {
        const hiddenCount = notifications.length - MAX_VISIBLE_NOTIFICATIONS;
        const lastVisibleNotification = notifications[MAX_VISIBLE_NOTIFICATIONS - 1].element;

        const counterBadge = document.createElement('div');
        counterBadge.className = 'notification-counter';
        counterBadge.textContent = `+${hiddenCount} more`;

        lastVisibleNotification.style.position = 'relative';
        lastVisibleNotification.appendChild(counterBadge);
    }
}

function removeNotification(id) {
    const notification = notifications.find(n => n.id === id);
    if (!notification) return;

    const element = notification.element;
    element.classList.remove('animate-slide-in');
    element.classList.add('animate-slide-out');

    element.addEventListener('animationend', () => {
        // Remove from our tracking array
        notifications = notifications.filter(n => n.id !== id);

        // Remove from DOM
        if (element.parentNode) {
            element.parentNode.removeChild(element);
        }

        // Update the display
        updateNotificationsDisplay();
    });
}

document.addEventListener('DOMContentLoaded', () => {
    // Add click event listeners to all notification buttons
    document.querySelectorAll('button[data-type]').forEach(button => {
        button.addEventListener('click', () => {
            const type = button.getAttribute('data-type');
            createNotification(type);
        });
    });

    if (typeof feather !== 'undefined') {
        feather.replace();
    }
});