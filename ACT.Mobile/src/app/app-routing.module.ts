import { NgModule } from '@angular/core';
import { PreloadAllModules, RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    loadChildren: () => import('./login/login.module').then( m => m.LoginPageModule)
  },
  {
    path: 'home',
    loadChildren: () => import('./home/home.module').then( m => m.HomePageModule)
  },
  {
    path: 'siteaudit',
    loadChildren: () => import('./siteaudit/siteaudit.module').then( m => m.SiteauditPageModule)
  },
  {
    path: 'outstandingreport',
    loadChildren: () => import('./outstandingreport/outstandingreport.module').then( m => m.OutstandingreportPageModule)
  },
  {
    path: 'podmanagement',
    loadChildren: () => import('./podmanagement/podmanagement.module').then( m => m.PodmanagementPageModule)
  },
  {
    path: 'notifications',
    loadChildren: () => import('./notifications/notifications.module').then( m => m.NotificationsPageModule)
  },
  {
    path: 'chat',
    loadChildren: () => import('./chat/chat.module').then( m => m.ChatPageModule)
  },
  {
    path: 'settings',
    loadChildren: () => import('./settings/settings.module').then( m => m.SettingsPageModule)
  },
  {
    path: 'forgotpassword',
    loadChildren: () => import('./forgotpassword/forgotpassword.module').then( m => m.ForgotpasswordPageModule)
  },
  {
    path: 'logout',
    loadChildren: () => import('./logout/logout.module').then( m => m.LogoutPageModule)
  },
  {
    path: 'outstandingpalletsperclient',
    loadChildren: () => import('./outstandingpalletsperclient/outstandingpalletsperclient.module').then( m => m.OutstandingpalletsperclientPageModule)
  },
  {
    path: 'outstandingpalletspersite',
    loadChildren: () => import('./outstandingpalletspersite/outstandingpalletspersite.module').then( m => m.OutstandingpalletspersitePageModule)
  },
  {
    path: 'notificationdetail',
    loadChildren: () => import('./notificationdetail/notificationdetail.module').then( m => m.NotificationdetailPageModule)
  },
  {
    path: 'siteauditdetail',
    loadChildren: () => import('./siteauditdetail/siteauditdetail.module').then( m => m.SiteauditdetailPageModule)
  },
  {
    path: 'addsiteaudit',
    loadChildren: () => import('./addsiteaudit/addsiteaudit.module').then( m => m.AddsiteauditPageModule)
  },
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, { preloadingStrategy: PreloadAllModules })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule
{

}
