# Food Ordering System API
A backend food ordering system built with ASP.NET Core Web API that supports authentication, menu management, cart, and order tracking.

This system is an online food ordering platform that allows users to browse menus, add meals to a cart, and place orders efficiently. It improves customer experience by reducing manual ordering and enabling digital tracking. The system supports both users and administrators with different access roles.

The process starts with authentication. Users log in, and the system validates their credentials before generating a secure token for access. After login, users can view the menu, which is retrieved dynamically from the database. They can select meals, adjust quantities, and add items to their cart. The system automatically calculates prices and stores cart data.

When the user confirms the order, the system creates an order with a default status of “Pending.” Administrators then monitor and update the order status to “Out for Delivery” or “Completed.” This ensures transparency, accountability, and efficient order management.

The flow begins with Start, which represents system entry.The User/Admin Login stage allows users to enter credentials.

The Validate User process checks correctness. If validation fails, the system moves to Invalid Password. If successful, a JWT Token is generated for secure communication.

Next, users access View Menu, where available meals are displayed. The decision Are You Ordering? checks user intent. If no, the system continues showing the menu.

If yes, users proceed to Add to Cart, selecting items and quantities. Another decision checks if the user wants to continue ordering. If no, the system moves to View Cart for review.

When confirmed, the order is created with status Pending, and the admin updates it to Out for Delivery, ending the process.

The system manages failures such as invalid login, expired tokens, network errors, and unavailable items. It prevents duplicate orders and unauthorized access. Input validation ensures correct data, while error messages guide users. Retry mechanisms handle temporary system failures.

To support large users, the system can use cloud hosting, load balancing, caching, and database optimization. A microservices architecture separates authentication, menu, and order services. Background processing improves performance, while monitoring and auto-scaling ensure reliability.
