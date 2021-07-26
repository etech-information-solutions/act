import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { OutstandingpalletspersitePage } from './outstandingpalletspersite.page';

const routes: Routes = [
  {
    path: '',
    component: OutstandingpalletspersitePage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OutstandingpalletspersitePageRoutingModule {}
