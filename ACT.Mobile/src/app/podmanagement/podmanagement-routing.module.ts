import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { PodmanagementPage } from './podmanagement.page';

const routes: Routes = [
  {
    path: '',
    component: PodmanagementPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PodmanagementPageRoutingModule {}
