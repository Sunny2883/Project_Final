import { DOCUMENT } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';
import { Router } from '@angular/router';
import 'jspdf-autotable';
import { ChangeDetectorRef } from '@angular/core';
import { ApiService } from '../services/api.service';
import { MatDialog } from '@angular/material/dialog';
import { ProfileModalComponent } from '../components/profile-modal/profile-modal.component';
import { AuthorizationService, Role } from '../services/authorization.service';
import { RoleEditModalComponent } from '../components/role-edit-modal/role-edit-modal.component';
import { environment } from '../../environments/environment.development';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
  userDetail!: any
  isLoading: boolean;
  constructor(public dialog: MatDialog, public router: Router, public apiService: ApiService, private cdr: ChangeDetectorRef, @Inject(DOCUMENT) public document: Document,
    public authService: AuthService, public authorizationService: AuthorizationService) {

    this.authService.user$.subscribe(userDetail => {
      this.userDetail = userDetail;
      console.log(this.userDetail);
      this.apiService.login({ email: this.userDetail.email, username: this.userDetail.sub }).subscribe(user => {
        user = JSON.parse(user);
        console.log(user);
        if (user.isSuccess == 1) {
          console.log(user);
          user = user.user
          this.apiService.showSuccessToast("User successfully logged in")
          localStorage.setItem("user", JSON.stringify(user));
          localStorage.setItem("role", JSON.stringify(user?.role));
          this.userDetail = { ...userDetail, roleId: user?.role };
        }
        else if (user.isSuccess == 2) {
          this.apiService.showSuccessToast("You are not verified yet")
          this.logout();
        }
        else {
          this.apiService.register({
            name: this.userDetail.nickname,
            username: this.userDetail.sub,
            email: this.userDetail.email
          }).subscribe(user => {
            console.log(user);

            this.apiService.showSuccessToast("Wait until the user is confirmed")
            this.logout();
          },
            error => {

            }
          );
        
        }
      },
        error => {
          console.log(error);
        }
      )
    });
    this.isLoading = false;
  }

 
  ngOnInit(): void {
    this.apiService.isLoading().subscribe(isLoading => {
      this.isLoading = isLoading;
    });
  }
  Navigations = [
    { path: 'dashboard/project', displayName: 'Project' },
    { path: 'dashboard/user-management', displayName: 'User Management' },
    { path: 'dashboard/role-management', displayName: 'Role Management' },
  ];

  isUserManagementPage() {
    const currentUrl = this.router.url
    return currentUrl != "/dashboard/user-management"
  }
  isProjectPage() {
    const currentUrl = this.router.url
    return currentUrl != "/dashboard/project"
  }
  isRolePage() {
    const currentUrl = this.router.url
    return currentUrl != "/dashboard/role-management"
  }
  logout() {
    this.authService.logout({
      logoutParams: {
        returnTo: `${environment.clientURL}login`
      }
    });
  }
  navigateTo(path: string) {
    // throw new Error('Method not implemented.');
    this.router.navigate([path])
  }


  isAdmin(): boolean {
    const userRole = this.authorizationService.getCurrentUser()?.role;
    return userRole === Role.Admin;
  }
  openDialog() {
    this.dialog.open(ProfileModalComponent);
  }

}
