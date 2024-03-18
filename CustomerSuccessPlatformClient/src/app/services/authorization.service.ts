import { Injectable } from '@angular/core';
export interface User {
  id: string;
  name: string;
  email: string;
  role: string;
}

export enum Role {
  Admin = 'Admin',
  Manager = 'Manager',
  Auditor = 'Auditor',
  Client = 'Client',
}

@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {


  private users: User[] = [
    { id: "1", name: 'Chintan Shah', email: 'chintan@example.com', role: 'admin' },
    { id: "2", name: 'Dipa Manjukar', email: 'dipa@example.com', role: 'manager' },
    { id: "2", name: 'Divyanshu Singh', email: 'divyanshu@example.com', role: 'auditor' },
    { id: "4", name: 'Rahul yadav', email: 'rahul@example.com', role: 'client' },
  ];

  roles!: Role[];


  hasRoles(requiredRoles: Role[]): boolean {
    return this.roles.some(role => requiredRoles.includes(role));
  }

  getAllUsers(): User[] {
    return this.users;
  }
  getCurrentUser(): any {
    let user = localStorage.getItem('user') as any;
    let role = localStorage.getItem('role') as any;

    if (role) {
      role = JSON.parse(role);
      user = JSON.parse(user);

      if (role) {
        let userRole = {
          name: user?.name,
          id: role?.id,
          email: user?.email,
          role: role?.name
        }
        return userRole
      }
      else {
        return { id: "4", name: 'Rahul yadav', email: 'rahul@example.com', role: 'client' }
      }
    }
  }




}
