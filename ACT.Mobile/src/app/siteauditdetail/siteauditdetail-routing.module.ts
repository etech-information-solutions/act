import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { SiteauditdetailPage } from './siteauditdetail.page';

const routes: Routes = [
  {
    path: '',
    component: SiteauditdetailPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SiteauditdetailPageRoutingModule {}
