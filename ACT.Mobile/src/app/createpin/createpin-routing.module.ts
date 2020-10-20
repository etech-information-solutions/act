import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { CreatepinPage } from './createpin.page';

const routes: Routes = [
  {
    path: '',
    component: CreatepinPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CreatepinPageRoutingModule {}
