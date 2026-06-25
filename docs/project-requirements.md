## Manual Dashboard Menu Rules for Demo 1

For Demo 1, menus will be manually created in `Views/Dashboard/Index.cshtml`.

Menu visibility will be decided using the logged-in user's `LevelNo`.

### Level 1 Users

Level 1 users can see:

- Apply Leave
- View Details / Personal Profile

Level 1 users cannot see:

- Approve Leave
- View Users

### Level 2, Level 3, and Level 4 Users

Level 2, Level 3, and Level 4 users can see:

- Apply Leave
- Approve Leave
- View Users
- View Details / Personal Profile

### View Users Rule

When a logged-in user clicks View Users, the system will show only employees who are directly working under that logged-in user.

The condition will be:

Users.ReportingAuthorityId = LoggedInUserId

For now, higher-level users can see only their immediate lower-level users.

Example:

- Level 2 user can see Level 1 users directly reporting to them.
- Level 3 user can see Level 2 users directly reporting to them.
- Level 4 user can see Level 3 users directly reporting to them.

Later, this can be enhanced with dropdown-based filtering to view Level 1 or Level 2 employees.