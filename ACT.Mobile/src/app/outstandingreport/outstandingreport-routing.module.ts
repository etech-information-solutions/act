import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { OutstandingreportPage } from './outstandingreport.page';

const routes: Routes = [
  {
    path: '',
    component: OutstandingreportPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OutstandingreportPageRoutingModule {}
