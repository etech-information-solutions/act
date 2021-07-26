import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { OutstandingshipmentdetailPage } from './outstandingshipmentdetail.page';

const routes: Routes = [
  {
    path: '',
    component: OutstandingshipmentdetailPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OutstandingshipmentdetailPageRoutingModule {}
