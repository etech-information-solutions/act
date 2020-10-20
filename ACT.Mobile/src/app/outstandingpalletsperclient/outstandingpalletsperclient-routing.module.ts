import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { OutstandingpalletsperclientPage } from './outstandingpalletsperclient.page';

const routes: Routes = [
  {
    path: '',
    component: OutstandingpalletsperclientPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OutstandingpalletsperclientPageRoutingModule {}
